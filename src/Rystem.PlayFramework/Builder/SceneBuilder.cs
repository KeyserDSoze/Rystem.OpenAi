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
        private readonly ActorsHandler _actorsHandler;
        public SceneBuilder(IServiceCollection services)
        {
            _services = services;
            _playHandler = _services.GetSingletonService<PlayHandler>()!;
            _functionsHandler = _services.GetSingletonService<FunctionsHandler>()!;
            _actorsHandler = _services.GetSingletonService<ActorsHandler>()!;
        }
        private static readonly Regex s_checkName = new("[^a-zA-Z0-9_\\-]{1,64}");
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
            var builderInstance = new ActorBuilder(_services, Scene.Name, _playHandler, _functionsHandler, _actorsHandler);
            builder(builderInstance);
            return this;
        }

        public ISceneBuilder UseMcpServer(string serverName, Action<IMcpServerToolFilterBuilder>? filterBuilder = null)
        {
            if (string.IsNullOrWhiteSpace(serverName))
                throw new ArgumentException("Server name cannot be empty", nameof(serverName));

            Scene.McpServerName = serverName;

            if (filterBuilder != null)
            {
                var builder = new McpServerToolFilterBuilder();
                filterBuilder(builder);
                Scene.McpSceneFilter = builder.Build();
            }
            else
            {
                // Default: all enabled
                Scene.McpSceneFilter = new McpSceneFilter();
            }

            return this;
        }

        private static readonly Regex s_regex = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$");
        public ISceneBuilder WithService<T>(Action<ISceneServiceBuilder<T>>? builder = null)
            where T : class
        {
            AddService(builder, _functionsHandler, Scene.Name, _playHandler);
            return this;
        }

        internal static void AddService<T>(Action<ISceneServiceBuilder<T>>? builder, FunctionsHandler functionsHandler, string? sceneName, PlayHandler? playHandler)
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
                if (playHandler is not null && sceneName is not null)
                    playHandler[sceneName].Functions.Add(functionName);
                var description = method.Description ?? (method.Info.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description ?? functionName;
                var jsonFunctionObject = new FunctionToolMainProperty();
                var jsonFunction = new FunctionTool
                {
                    Name = functionName,
                    Description = description,
                    Parameters = jsonFunctionObject
                };
                var function = functionsHandler[functionName];
                function.Description = description; // Store description for planner
                if (sceneName is not null)
                    function.Scenes.Add(sceneName);
                else
                    function.ForEveryScene = true;
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
                        else if (result is ValueTask valueTask)
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
                            try
                            {
                                // Special handling for DateOnly and TimeOnly - use JsonSerializer with converters
                                if (parameterType == typeof(DateOnly) || parameterType == typeof(DateOnly?))
                                {
                                    if (DateOnly.TryParse(value[parameterName], out var dateOnlyValue))
                                        bringer.Parameters.Add(dateOnlyValue);
                                    else
                                        throw new FormatException($"Value '{value[parameterName]}' is not a valid DateOnly format.");
                                }
                                else if (parameterType == typeof(TimeOnly) || parameterType == typeof(TimeOnly?))
                                {
                                    if (TimeOnly.TryParse(value[parameterName], out var timeOnlyValue))
                                        bringer.Parameters.Add(timeOnlyValue);
                                    else
                                        throw new FormatException($"Value '{value[parameterName]}' is not a valid TimeOnly format.");
                                }
                                else if (parameterType.IsEnum)
                                {
                                    bringer.Parameters.Add(Enum.Parse(parameterType, value[parameterName], true).Cast(parameterType));
                                }
                                else if (parameterType.IsPrimitive())
                                {
                                    if (parameterType == typeof(DateTime))
                                        bringer.Parameters.Add(DateTime.Parse(value[parameterName]));
                                    else
                                        bringer.Parameters.Add(value[parameterName].Cast(parameterType));
                                }
                                else
                                    bringer.Parameters.Add(JsonSerializer.Deserialize(value[parameterName], parameterType, s_options)!);
                            }
                            catch (JsonException ex)
                            {
                                // If JSON deserialization fails, throw a meaningful exception
                                throw new InvalidOperationException(
                                    $"Failed to deserialize parameter '{parameterName}' of type '{parameterType.Name}'. " +
                                    $"Value: '{value[parameterName]}'. " +
                                    $"Error: {ex.Message}", ex);
                            }
                            catch (FormatException ex)
                            {
                                throw new InvalidOperationException(
                                    $"Invalid format for parameter '{parameterName}' of type '{parameterType.Name}'. " +
                                    $"Value: '{value[parameterName]}'. " +
                                    $"Error: {ex.Message}", ex);
                            }
                            return ValueTask.CompletedTask;
                        });
                }
            }
        }

        private static readonly JsonSerializerOptions s_options = new()
        {
            Converters =
            {
                new FlexibleEnumConverterFactory(),
                new Converters.LenientDateOnlyConverter(),
                new Converters.LenientNullableDateOnlyConverter(),
                new Converters.LenientTimeOnlyConverter(),
                new Converters.LenientNullableTimeOnlyConverter(),
            },
        };
        private sealed class FlexibleEnumConverterFactory : JsonConverterFactory
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
        private sealed class FlexibleEnumConverter<T> : JsonConverter<T> where T : struct, Enum
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var strValue = reader.GetString();
                    if (int.TryParse(strValue, out var intValue))
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
