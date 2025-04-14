using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi;

namespace Rystem.PlayFramework
{
    internal sealed class SceneBuilder : ISceneBuilder
    {
        private readonly IServiceCollection _services;
        internal IScene Scene { get; } = new Scene();
        private readonly PlayHandler _playHandler;
        private readonly FunctionsHandler _functionsHandler;
        public SceneBuilder(IServiceCollection services)
        {
            _services = services;
            _playHandler = _services.GetSingletonService<PlayHandler>()!;
            _functionsHandler = _services.GetSingletonService<FunctionsHandler>()!;
        }
        private static readonly Regex s_checkName = new("[^a-zA-Z0-9_-]{1,64}");
        public ISceneBuilder WithName(string name)
        {
            Scene.Name = s_checkName.Replace(name.Replace(' ', '-'), string.Empty);
            if (Scene.Name.Length > 64)
                Scene.Name = Scene.Name[..64];
            return this;
        }
        public ISceneBuilder WithDescription(string description)
        {
            Scene.Description = description;
            return this;
        }
        public ISceneBuilder WithOpenAi(string name)
        {
            Scene.OpenAiFactoryName = name;
            return this;
        }
        public ISceneBuilder WithHttpClient(string name)
        {
            Scene.HttpClientName = name;
            return this;
        }
        public ISceneBuilder WithApi(Action<IScenePathBuilder> builder)
        {
            var scenePathBuilder = new ScenePathBuilder();
            builder(scenePathBuilder);
            _playHandler[Scene.Name].AvailableApiPath.AddRange(scenePathBuilder.RegexForApiMapping);
            return this;
        }
        public ISceneBuilder WithActors(Action<IActorBuilder> builder)
        {
            var builderInstance = new ActorBuilder(_services, Scene.Name, _playHandler, _functionsHandler);
            builder(builderInstance);
            return this;
        }
        private static readonly Regex s_regex = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$");
        public ISceneBuilder WithService<T>(Action<ISceneServiceBuilder<T>>? builder = null)
            where T : class
        {
            var methods = new List<MethodBringer>();
            var currentType = typeof(T);
            if (builder == null)
            {
                methods.AddRange(currentType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(x => new MethodBringer(x, null)));
            }
            else
            {
                var sceneServiceBuilder = new SceneServiceBuilder<T>();
                builder(sceneServiceBuilder);
                methods.AddRange(sceneServiceBuilder.Methods);
            }
            var serviceName = typeof(T).Name;
            foreach (var method in methods)
            {
                var functionName = method.Name ?? s_regex.Replace($"{serviceName}_{currentType.Name}_{method.Info.Name}", string.Empty);
                _playHandler[Scene.Name].Functions.Add(functionName);
                var description = method.Description ?? (method.Info.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description ?? functionName;
                var jsonFunctionObject = new FunctionToolMainProperty();
                var jsonFunction = new FunctionTool
                {
                    Name = functionName,
                    Description = description,
                    Parameters = jsonFunctionObject
                };
                var function = _functionsHandler[functionName];
                function.Scenes.Add(Scene.Name);
                function.Chooser = x => x.AddFunctionTool(jsonFunction);
                var withoutReturn = method.Info.ReturnType == typeof(void) || method.Info.ReturnType == typeof(Task) || method.Info.ReturnType == typeof(ValueTask);
                var isGenericAsync = method.Info.ReturnType.IsGenericType &&
                    (method.Info.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)
                    || method.Info.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>));
                var hasCancellationToken = method.Info.GetParameters().Any(x => x.ParameterType == typeof(CancellationToken));
                function.Service = new()
                {
                    Call = async (serviceProvider, bringer, sceneContext, cancellationToken) =>
                    {
                        var service = serviceProvider.GetRequiredService<T>();
                        var result = hasCancellationToken ?
                            method.Info.Invoke(service, [.. bringer.Parameters, cancellationToken]) :
                            method.Info.Invoke(service, [.. bringer.Parameters]);
                        if (result is Task task)
                            await task;
                        if (result is ValueTask valueTask)
                            await valueTask;
                        if (withoutReturn)
                            return default!;
                        else if (result is not null)
                        {
                            var response = isGenericAsync ? ((dynamic)result!).Result : result;
                            if (response is not null)
                                return response;
                            else
                                return default!;
                        }
                        else
                            return default!;
                    }
                };
                foreach (var parameter in method.Info.GetParameters())
                {
                    if (parameter.ParameterType == typeof(CancellationToken))
                        continue;
                    var parameterName = parameter.Name ?? parameter.ParameterType.Name;
                    ToolPropertyHelper.Add(parameterName, parameter.ParameterType, jsonFunctionObject, null);
                    if (!parameter.IsNullable())
                        jsonFunctionObject.AddRequired(parameterName);
                    var parametersFiller = function.Service.Actions;
                    var parameterType = parameter.ParameterType;
                    if (!parametersFiller.ContainsKey(parameterName))
                        parametersFiller.Add(parameterName, (value, bringer) =>
                        {
                            if (parameterType.IsPrimitive())
                                bringer.Parameters.Add(value[parameterName].Cast(parameterType));
                            else
                                bringer.Parameters.Add(JsonSerializer.Deserialize(value[parameterName], parameterType, s_options)!);
                            return ValueTask.CompletedTask;
                        });
                }
            }
            return this;
        }
        private static readonly JsonSerializerOptions s_options = new()
        {
            Converters =
            {
                new FlexibleEnumConverterFactory(),
            },
        };
        public class FlexibleEnumConverterFactory : JsonConverterFactory
        {
            public override bool CanConvert(Type typeToConvert)
            {
                return typeToConvert.IsEnum;
            }

            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            {
                var converterType = typeof(FlexibleEnumConverter<>).MakeGenericType(typeToConvert);
                return (JsonConverter)Activator.CreateInstance(converterType)!;
            }
        }
        public class FlexibleEnumConverter<T> : JsonConverter<T> where T : struct, Enum
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var strValue = reader.GetString();
                    if (int.TryParse(strValue, out int intValue))
                    {
                        return (T)Enum.ToObject(typeof(T), intValue);
                    }

                    if (Enum.TryParse<T>(strValue, ignoreCase: true, out var result))
                    {
                        return result;
                    }

                    throw new JsonException($"Unable to convert \"{strValue}\" to Enum {typeof(T)}.");
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    var intValue = reader.GetInt32();
                    return (T)Enum.ToObject(typeof(T), intValue);
                }

                throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum.");
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(Convert.ToInt32(value));
            }
        }

    }
}
