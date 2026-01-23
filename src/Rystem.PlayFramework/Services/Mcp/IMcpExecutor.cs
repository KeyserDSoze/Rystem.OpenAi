namespace Rystem.PlayFramework
{
    /// <summary>
    /// Interface for executing MCP tool calls
    /// </summary>
    public interface IMcpExecutor
    {
        /// <summary>
        /// Executes an MCP tool with the given arguments
        /// </summary>
        /// <param name="mcpCall">Information about the MCP tool to call</param>
        /// <param name="argumentsJson">Arguments as JSON string</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result as JSON string</returns>
        /// <exception cref="InvalidOperationException">If tool execution fails</exception>
        Task<string> ExecuteToolAsync(McpToolCall mcpCall, string argumentsJson, CancellationToken cancellationToken);
    }
}
