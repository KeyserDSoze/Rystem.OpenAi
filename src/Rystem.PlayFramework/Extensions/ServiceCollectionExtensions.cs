using Rystem.PlayFramework;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlayFramework(this IServiceCollection services,
            Action<IScenesBuilder> builder,
            Action<IServiceBuilder>? serviceBuilder = null,
            string? name = null,
            bool streaming = false)
        {
            services.AddPopulationService();
            services.AddHttpContextAccessor();
            if (!streaming)
                services.AddFactory<ISceneManager, SceneManager>(name, ServiceLifetime.Transient);
            else
                services.AddFactory<ISceneManager, SceneManager>(name, ServiceLifetime.Transient);
            //services.AddFactory<ISceneManager, StreamingSceneManager>(name, ServiceLifetime.Transient);
            services.AddSingleton<ActorsOpenAiEndpointParser>();
            services.AddSingleton(new FunctionsHandler());
            services.AddSingleton(new PlayHandler());

            // Register MCP services
            services.AddSingleton<McpRegistry>();
            services.AddSingleton<McpClientFactory>();
            services.AddScoped<IMcpExecutor, McpToolExecutor>();

            // Register deterministic planner, summarizer and response parser
            var serviceBuilderInstance = new ServiceBuilder(services);
            serviceBuilder?.Invoke(serviceBuilderInstance);
            serviceBuilderInstance.AddDefaultsIfNeeded();

            var sceneBuilder = new ScenesBuilder(services);
            sceneBuilder.AddCustomDirector<MainDirector>();
            builder(sceneBuilder);

            return services;
        }
    }
}
