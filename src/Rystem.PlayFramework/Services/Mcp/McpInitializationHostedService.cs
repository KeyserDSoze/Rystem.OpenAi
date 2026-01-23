using Microsoft.Extensions.Hosting;

namespace Rystem.PlayFramework
{
    /// <summary>
    /// Hosted service for initializing MCP servers on application startup
    /// </summary>
    internal sealed class McpInitializationHostedService : IHostedService
    {
        private readonly ScenesBuilder _scenesBuilder;

        public McpInitializationHostedService(ScenesBuilder scenesBuilder)
        {
            _scenesBuilder = scenesBuilder ?? throw new ArgumentNullException(nameof(scenesBuilder));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // MCP servers will be initialized when first accessed
            // This is intentionally lazy-loaded to avoid issues with dependency resolution
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Cleanup will happen via IAsyncDisposable on McpRegistry
            await Task.CompletedTask;
        }
    }
}
