using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rystem.OpenAi;
using Rystem.PlayFramework.Mcp.Server;

namespace Rystem.PlayFramework
{
    internal sealed class ScenesBuilder : IScenesBuilder
    {
        private readonly IServiceCollection _services;
        private readonly SceneManagerSettings _settings;
        private readonly PlayHandler _playHander;
        private readonly FunctionsHandler _functionsHandler;
        private readonly ActorsHandler _actorsHandler;
        private readonly Dictionary<string, McpServerConfiguration> _mcpServers = [];
        private ExposeAsMcpServerConfig? _exposeConfig;

        public ScenesBuilder(IServiceCollection services)
        {
            _services = services;
            _settings = new();
            _playHander = _services.GetSingletonService<PlayHandler>()!;
            _functionsHandler = _services.GetSingletonService<FunctionsHandler>()!;
            _actorsHandler = _services.GetSingletonService<ActorsHandler>()!;
        }
        public const string MainActor = nameof(MainActor);
        public const string Request = nameof(Request);
        public IScenesBuilder Configure(Action<SceneManagerSettings> settings)
        {
            settings(_settings);
            _services.TryAddSingleton(_settings);
            return this;
        }
        public IScenesBuilder AddMainActor(string role, bool playInEveryScene)
        {
            if (playInEveryScene)
                _services.AddKeyedSingleton<IPlayableActor>(MainActor, new SimpleActor { Role = role });
            else
                _services.AddKeyedSingleton<IActor>(MainActor, new SimpleActor { Role = role });
            _actorsHandler.AddActorInfo(MainActor, role);
            return this;
        }
        public IScenesBuilder AddMainActor<T>(bool playInEveryScene)
            where T : class, IActor
        {
            if (playInEveryScene)
                _services.AddKeyedTransient<IPlayableActor, T>(MainActor);
            else
                _services.AddKeyedTransient<IActor, T>(MainActor);
            _actorsHandler.AddActorInfo(MainActor, null, typeof(T).Name);
            return this;
        }
        public IScenesBuilder AddMainActor(Func<SceneContext, string> action, bool playInEveryScene)
        {
            if (playInEveryScene)
                _services.AddKeyedSingleton<IPlayableActor>(MainActor, new ActionActor { Action = action });
            else
                _services.AddKeyedSingleton<IActor>(MainActor, new ActionActor { Action = action });
            _actorsHandler.AddActorInfo(MainActor, "Dynamic main action actor");
            return this;
        }
        public IScenesBuilder AddMainActor(Func<SceneContext, CancellationToken, Task<string>> action, bool playInEveryScene)
        {
            if (playInEveryScene)
                _services.AddKeyedSingleton<IPlayableActor>(MainActor, new AsyncActionActor { Action = action });
            else
                _services.AddKeyedSingleton<IActor>(MainActor, new AsyncActionActor { Action = action });
            _actorsHandler.AddActorInfo(MainActor, "Async dynamic main action actor");
            return this;
        }
        public IScenesBuilder AddCustomDirector<T>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where T : class, IDirector
        {
            _services.RemoveAll<IDirector>();
            _services.AddService<IDirector, T>(lifetime);
            return this;
        }
        public IScenesBuilder AddCustomSummarizer<T>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where T : class, ISummarizer
        {
            _services.RemoveAll<ISummarizer>();
            _services.AddService<ISummarizer, T>(lifetime);
            return this;
        }
        public IScenesBuilder AddScene(Action<ISceneBuilder> builder)
        {
            var sceneBuilder = new SceneBuilder(_services);
            builder(sceneBuilder);
            _services.AddFactory(sceneBuilder.Scene, sceneBuilder.Scene.Name, ServiceLifetime.Singleton);
            _playHander[sceneBuilder.Scene.Name].Chooser = x => x.AddFunctionTool(new FunctionTool
            {
                Name = sceneBuilder.Scene.Name,
                Description = sceneBuilder.Scene.Description,
            });
            return this;
        }

        public IScenesBuilder AddCommonService<T>(Action<ISceneServiceBuilder<T>>? builder = null)
           where T : class
        {
            SceneBuilder.AddService(builder, _functionsHandler, null, null);
            return this;
        }

        public IScenesBuilder AddCache(Action<ICacheBuilder> cacheBuilder)
        {
            var builder = new CacheBuilder(_services);
            cacheBuilder(builder);
            return this;
        }

        public IScenesBuilder AddMcpServer(string serverName, Action<IMcpServerBuilder> builder)
        {
            if (string.IsNullOrWhiteSpace(serverName))
                throw new ArgumentException("Server name cannot be empty", nameof(serverName));

            if (_mcpServers.ContainsKey(serverName))
                throw new InvalidOperationException($"MCP server '{serverName}' is already registered");

            var mcpBuilder = new McpServerBuilder();
            builder(mcpBuilder);
            var config = mcpBuilder.Build();

            _mcpServers[serverName] = config;
            return this;
        }

        internal async Task InitializeMcpServersAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            if (_mcpServers.Count == 0)
                return;

            var registry = serviceProvider.GetRequiredService<McpRegistry>();
            var factory = serviceProvider.GetRequiredService<McpClientFactory>();

            foreach (var (serverName, config) in _mcpServers)
            {
                try
                {
                    // Create appropriate client
                    IMcpClient client = !string.IsNullOrEmpty(config.HttpUrl)
                        ? factory.CreateHttpClient(config.HttpUrl)
                        : factory.CreateStdioClient(config.Command!, config.CommandArgs ?? []);

                    // Register in registry (ONE TIME!)
                    registry.RegisterServer(serverName, client);

                    // Connect
                    await client.ConnectAsync(cancellationToken);

                    // Note: Tools/Resources/Prompts are NOT discovered here!
                    // They are discovered on-demand in SceneManager.GetResponseFromSceneAsync()
                    // based on each scene's filter configuration
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to initialize MCP server '{serverName}': {ex.Message}", ex);
                }
            }
        }

        public IScenesBuilder ExposeAsMcpServer(Action<ExposeAsMcpServerConfig>? configure = null)
        {
            _exposeConfig = new ExposeAsMcpServerConfig();
            configure?.Invoke(_exposeConfig);
            return this;
        }

        internal ExposeAsMcpServerConfig? GetExposeConfig() => _exposeConfig;
    }
}
