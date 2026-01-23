using Rystem.OpenAi;

namespace Rystem.PlayFramework
{
    /// <summary>
    /// Interface for communicating with an MCP (Model Context Protocol) server
    /// </summary>
    public interface IMcpClient : IAsyncDisposable
    {
        /// <summary>
        /// Establishes connection with the MCP server
        /// </summary>
        Task ConnectAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Closes connection with the MCP server
        /// </summary>
        Task DisconnectAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Indicates whether the client is currently connected to the server
        /// </summary>
        bool IsConnected { get; }

        // === TOOLS ===

        /// <summary>
        /// Gets the list of available tools from the MCP server
        /// </summary>
        Task<IReadOnlyList<FunctionTool>> ListToolsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Calls a specific tool on the MCP server
        /// </summary>
        Task<string> CallToolAsync(string toolName, Dictionary<string, object> arguments, CancellationToken cancellationToken);

        // === RESOURCES ===

        /// <summary>
        /// Gets the list of available resources from the MCP server
        /// </summary>
        Task<IReadOnlyList<McpResource>> ListResourcesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Reads the content of a specific resource
        /// </summary>
        Task<McpResourceContent> ReadResourceAsync(string uri, CancellationToken cancellationToken);

        // === PROMPTS ===

        /// <summary>
        /// Gets the list of available prompts from the MCP server
        /// </summary>
        Task<IReadOnlyList<McpPrompt>> ListPromptsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets a specific prompt with optional arguments
        /// </summary>
        Task<McpPromptContent> GetPromptAsync(string promptName, Dictionary<string, string>? arguments, CancellationToken cancellationToken);
    }
}
