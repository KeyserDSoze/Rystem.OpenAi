﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.PlayFramework
{
    public interface IScenesBuilder
    {
        IScenesBuilder Configure(Action<SceneManagerSettings> settings);
        IScenesBuilder AddMainActor(string role, bool playInEveryScene);
        IScenesBuilder AddMainActor<T>(bool playInEveryScene) where T : class, IActor;
        IScenesBuilder AddMainActor(Func<SceneContext, string> action, bool playInEveryScene);
        IScenesBuilder AddMainActor(Func<SceneContext, CancellationToken, Task<string>> action, bool playInEveryScene);
        IScenesBuilder AddCustomDirector<T>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, IDirector;
        IScenesBuilder AddScene(Action<ISceneBuilder> builder);
        IScenesBuilder AddCache(Action<ICacheBuilder> cacheBuilder);
    }
}
