using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi;

namespace Rystem.PlayFramework
{
    internal sealed class SceneBuilder : ISceneBuilder
    {
        private readonly IServiceCollection _services;
        internal IScene Scene { get; } = new Scene();
        private readonly PlayHandler _playHander;
        private readonly FunctionsHandler _functionsHandler;
        public SceneBuilder(IServiceCollection services)
        {
            _services = services;
            _playHander = _services.GetSingletonService<PlayHandler>()!;
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
            _playHander[Scene.Name].AvailableApiPath.AddRange(scenePathBuilder.RegexForApiMapping);
            return this;
        }
        public ISceneBuilder WithActors(Action<IActorBuilder> builder)
        {
            var builderInstance = new ActorBuilder(_services, Scene.Name);
            builder(builderInstance);
            return this;
        }
        public ISceneBuilder WithService<T>(Action<ISceneServiceBuilder<T>>? builder = null)
            where T : class
        {
            var methods = new List<MethodInfo>();
            var currentType = typeof(T);
            if (builder == null)
            {
                methods.AddRange(currentType.GetMethods(BindingFlags.Public | BindingFlags.Instance));
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
                var functionName = $"{serviceName}_{currentType.Name}_{method.Name}";
                _playHander[Scene.Name].Functions.Add(functionName);
                var description = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(DescriptionAttribute)) as DescriptionAttribute;
                var jsonFunctionObject = new FunctionToolMainProperty();
                var jsonFunction = new FunctionTool
                {
                    Name = functionName,
                    Description = description?.Description ?? functionName,
                    Parameters = jsonFunctionObject
                };
                var function = _functionsHandler[functionName];
                function.Scenes.Add(Scene.Name);
                function.Chooser = x => x.AddFunctionTool(jsonFunction);
                var withoutReturn = method.ReturnType == typeof(void) || method.ReturnType == typeof(Task) || method.ReturnType == typeof(ValueTask);
                var isGenericAsync = method.ReturnType.IsGenericType &&
                    (method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)
                    || method.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>));
                var hasCancellationToken = method.GetParameters().Any(x => x.ParameterType == typeof(CancellationToken));
                function.Service = new()
                {
                    Call = async (serviceProvider, bringer, cancellationToken) =>
                    {
                        var service = serviceProvider.GetRequiredService<T>();
                        var result = hasCancellationToken ?
                            method.Invoke(service, [.. bringer.Parameters, cancellationToken]) :
                            method.Invoke(service, [.. bringer.Parameters]);
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
                foreach (var parameter in method.GetParameters())
                {
                    if (parameter.ParameterType == typeof(CancellationToken))
                        continue;
                    var parameterName = parameter.Name ?? parameter.ParameterType.Name;
                    ToolPropertyHelper.Add(parameterName, parameter.ParameterType, jsonFunctionObject);
                    if (!parameter.IsNullable())
                        jsonFunctionObject.AddRequired(parameterName);
                    var parametersFiller = function.Service.Actions;
                    if (!parametersFiller.ContainsKey(parameterName))
                        parametersFiller.Add(parameterName, (value, bringer) =>
                        {
                            bringer.Parameters.Add(value[parameterName]);
                            return ValueTask.CompletedTask;
                        });
                }
            }
            return this;
        }
    }
}
