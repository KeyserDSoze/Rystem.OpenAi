using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.PlayFramework
{
    internal sealed class ActorBuilder : IActorBuilder
    {
        private readonly IServiceCollection _services;
        private readonly string _sceneName;
        public ActorBuilder(IServiceCollection services, string sceneName)
        {
            _services = services;
            _sceneName = sceneName;
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
