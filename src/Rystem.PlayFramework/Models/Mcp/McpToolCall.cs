namespace Rystem.PlayFramework
{
    /// <summary>
    /// Information needed to execute an MCP tool
    /// </summary>
    public sealed class McpToolCall
    {
        /// <summary>
        /// Name of the MCP server (e.g., "weather-server", "search-server")
        /// </summary>
        public required string ServerName { get; init; }

        /// <summary>
        /// Name of the tool to call (e.g., "get_weather")
        /// </summary>
        public required string ToolName { get; init; }

        /// <summary>
        /// The MCP client instance to use for execution
        /// </summary>
        public required IMcpClient Client { get; init; }
    }
}
