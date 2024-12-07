using Rystem.PlayFramework;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlayFramework(this IServiceCollection services,
            Action<IScenesBuilder> builder, string? name = null)
        {
            services.AddPopulationService();
            services.AddHttpContextAccessor();
            services.AddFactory<ISceneManager, SceneManager>(name, ServiceLifetime.Transient);
            services.AddSingleton<ActorsOpenAiEndpointParser>();
            services.AddSingleton(new FunctionsHandler());
            services.AddSingleton(new PlayHandler());
            var sceneBuilder = new ScenesBuilder(services);
            sceneBuilder.AddCustomDirector<MainDirector>();
            builder(sceneBuilder);
            return services;
        }
    }
}
