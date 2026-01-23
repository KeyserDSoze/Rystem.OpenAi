namespace Rystem.PlayFramework
{
    /// <summary>
    /// Fluent builder for configuring an MCP server globally.
    /// Scenes decide which elements to use via UseMcpServer().
    /// </summary>
    public interface IMcpServerBuilder
    {
        /// <summary>
        /// Configures the MCP server to use HTTP communication
        /// </summary>
        /// <param name="serverUrl">The HTTP URL of the MCP server (e.g., "http://localhost:3000")</param>
        IMcpServerBuilder WithHttpServer(string serverUrl);

        /// <summary>
        /// Configures the MCP server to use stdio communication with a command
        /// </summary>
        /// <param name="executable">The executable command or path</param>
        /// <param name="args">Arguments to pass to the command</param>
        IMcpServerBuilder WithCommand(string executable, params string[] args);

        /// <summary>
        /// Specifies the timeout for MCP server calls
        /// </summary>
        IMcpServerBuilder WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Internal method to build the configuration
        /// </summary>
        internal McpServerConfiguration Build();
    }
}
