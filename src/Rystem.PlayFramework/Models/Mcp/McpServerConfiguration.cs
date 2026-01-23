namespace Rystem.PlayFramework
{
    /// <summary>
    /// Configuration for an MCP server registration.
    /// Scenes decide which elements to use via UseMcpServer().
    /// </summary>
    public sealed class McpServerConfiguration
    {
        /// <summary>
        /// HTTP URL of the MCP server (for HTTP-based communication)
        /// </summary>
        public string? HttpUrl { get; init; }

        /// <summary>
        /// Command to execute for stdio-based MCP server
        /// </summary>
        public string? Command { get; init; }

        /// <summary>
        /// Arguments for the command
        /// </summary>
        public string[]? CommandArgs { get; init; }

        /// <summary>
        /// Timeout for MCP server calls
        /// </summary>
        public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
    }
}
