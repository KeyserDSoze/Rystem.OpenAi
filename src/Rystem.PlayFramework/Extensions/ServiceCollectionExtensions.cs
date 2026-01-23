using Rystem.PlayFramework;
using Rystem.PlayFramework.Mcp.Server;
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

            // Register MCP Client services
            services.AddSingleton<McpRegistry>();
            services.AddSingleton<McpClientFactory>();
            services.AddScoped<IMcpExecutor, McpToolExecutor>();

            // Register MCP Server services (for exposing PlayFramework as MCP server)
            services.AddSingleton<PlayFrameworkMcpServerRegistry>();
            services.AddSingleton<PlayFrameworkDocumentationBuilder>();
            services.AddSingleton<IMcpMethodHandler, ToolsListHandler>();
            services.AddSingleton<IMcpMethodHandler, ToolsCallHandler>();
            services.AddSingleton<IMcpMethodHandler, ResourcesListHandler>();
            services.AddSingleton<IMcpMethodHandler, ResourcesReadHandler>();
            services.AddSingleton<IMcpMethodHandler, PromptsListHandler>();
            services.AddSingleton<IMcpMethodHandler, PromptsGetHandler>();
            services.AddSingleton<McpMethodRouter>();

            // Register deterministic planner, summarizer and response parser
            var serviceBuilderInstance = new ServiceBuilder(services);
            serviceBuilder?.Invoke(serviceBuilderInstance);
            serviceBuilderInstance.AddDefaultsIfNeeded();

            var sceneBuilder = new ScenesBuilder(services);
            sceneBuilder.AddCustomDirector<MainDirector>();
            builder(sceneBuilder);

            // If ExposeAsMcpServer was called, register in the registry
            var exposeConfig = sceneBuilder.GetExposeConfig();
            if (exposeConfig != null)
            {
                var playFrameworkName = name ?? "default";
                services.AddSingleton(sp =>
                {
                    var registry = sp.GetRequiredService<PlayFrameworkMcpServerRegistry>();
                    var sceneFactory = sp.GetRequiredService<IFactory<IScene>>();

                    // Build scene documentation from registered scenes
                    var sceneDocs = new List<SceneDocumentation>();
                    // Note: Scene documentation will be populated during initialization
                    // For now, we register with basic info

                    var info = new ExposedPlayFrameworkInfo
                    {
                        Name = playFrameworkName,
                        Description = exposeConfig.Description,
                        Prompt = exposeConfig.Prompt,
                        EnableResources = exposeConfig.EnableResources,
                        AuthorizationPolicy = exposeConfig.AuthorizationPolicy,
                        SceneDocumentations = sceneDocs
                    };

                    registry.Register(info);
                    return info;
                });
            }

            return services;
        }
    }
}
