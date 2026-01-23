namespace Rystem.PlayFramework
{
    /// <summary>
    /// Registry for managing all MCP server clients
    /// </summary>
    internal sealed class McpRegistry : IAsyncDisposable
    {
        private readonly Dictionary<string, IMcpClient> _servers = [];

        /// <summary>
        /// Registers a new MCP server client
        /// </summary>
        public void RegisterServer(string serverName, IMcpClient client)
        {
            if (_servers.ContainsKey(serverName))
                throw new InvalidOperationException($"MCP server '{serverName}' is already registered");

            _servers[serverName] = client;
        }

        /// <summary>
        /// Gets a registered MCP server client
        /// </summary>
        public IMcpClient? GetClient(string serverName)
        {
            return _servers.TryGetValue(serverName, out var client) ? client : null;
        }

        /// <summary>
        /// Gets all registered server names
        /// </summary>
        public IEnumerable<string> GetServerNames() => _servers.Keys;

        /// <summary>
        /// Connects all registered MCP servers
        /// </summary>
        public async Task ConnectAllAsync(CancellationToken cancellationToken)
        {
            var tasks = _servers.Values.Select(client => client.ConnectAsync(cancellationToken));
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Disconnects all registered MCP servers
        /// </summary>
        public async Task DisconnectAllAsync(CancellationToken cancellationToken)
        {
            var tasks = _servers.Values.Select(client => client.DisconnectAsync(cancellationToken));
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Cleanup on disposal
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisconnectAllAsync(CancellationToken.None);

            foreach (var client in _servers.Values)
            {
                await client.DisposeAsync();
            }

            _servers.Clear();
        }
    }
}
