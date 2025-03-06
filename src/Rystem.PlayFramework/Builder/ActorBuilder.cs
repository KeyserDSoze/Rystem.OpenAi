using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi;

namespace Rystem.PlayFramework
{
    internal sealed class ActorBuilder : IActorBuilder
    {
        private readonly IServiceCollection _services;
        private readonly string _sceneName;
        private readonly PlayHandler _playHandler;
        private readonly FunctionsHandler _functionsHandler;

        public ActorBuilder(IServiceCollection services, string sceneName, PlayHandler playHandler, FunctionsHandler functionsHandler)
        {
            _services = services;
            _sceneName = sceneName;
            _playHandler = playHandler;
            _functionsHandler = functionsHandler;
        }
        public IActorBuilder AddActor<T>()
            where T : class, IActor
        {
            _services.AddKeyedTransient<IActor, T>(_sceneName);
            return this;
        }
        public IActorBuilder AddActor(string role)
        {
            _services.AddKeyedSingleton<IActor>(_sceneName, new SimpleActor { Role = role });
            return this;
        }
        public IActorBuilder AddActor<T>(string name, string role, Action<Dictionary<string, string>> parameters)
            where T : class, IActor
        {
            _services.AddKeyedTransient<T>(_sceneName);
            _playHandler[_sceneName].Functions.Add(name);
            var description = role;
            var jsonFunctionObject = new FunctionToolMainProperty();
            var jsonFunction = new FunctionTool
            {
                Name = name,
                Description = role,
                Parameters = jsonFunctionObject
            };
            var function = _functionsHandler[name];
            function.Scenes.Add(_sceneName);
            function.Chooser = x => x.AddFunctionTool(jsonFunction);
            var dictionary = new Dictionary<string, string>();
            parameters.Invoke(dictionary);
            foreach (var parameter in dictionary)
            {
                var parameterName = parameter.Key;
                ToolPropertyHelper.Add(parameterName, typeof(string), jsonFunctionObject, parameter.Value);
                jsonFunctionObject.AddRequired(parameterName);
            }
            function.Service = new()
            {
                Call = async (serviceProvider, bringer, sceneContext, cancellationToken) =>
                {
                    var actorService = serviceProvider.GetRequiredKeyedService<T>(_sceneName);
                    var parameters = bringer.Parameters;
                    return await actorService.PlayAsync(sceneContext, cancellationToken);
                }
            };
            return this;
        }
        public IActorBuilder AddActor(Func<SceneContext, string> action)
        {
            _services.AddKeyedSingleton<IActor>(_sceneName, new ActionActor { Action = action });
            return this;
        }
        public IActorBuilder AddActor(Func<SceneContext, CancellationToken, Task<string>> action)
        {
            _services.AddKeyedSingleton<IActor>(_sceneName, new AsyncActionActor { Action = action });
            return this;
        }
    }
}
