namespace Rystem.PlayFramework
{
    /// <summary>
    /// Factory for creating MCP client instances
    /// </summary>
    internal sealed class McpClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public McpClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Creates an HTTP-based MCP client
        /// </summary>
        public IMcpClient CreateHttpClient(string serverUrl)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
                throw new ArgumentException("Server URL cannot be empty", nameof(serverUrl));

            if (!Uri.TryCreate(serverUrl, UriKind.Absolute, out _))
                throw new ArgumentException($"Invalid server URL: {serverUrl}", nameof(serverUrl));

            return new HttpMcpClient(serverUrl, _httpClientFactory);
        }

        /// <summary>
        /// Creates a stdio-based MCP client (future implementation)
        /// </summary>
        public IMcpClient CreateStdioClient(string command, string[] args)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Command cannot be empty", nameof(command));

            // TODO: Implement StdioMcpClient when needed
            throw new NotImplementedException("Stdio MCP client is not yet implemented");
        }
    }
}
