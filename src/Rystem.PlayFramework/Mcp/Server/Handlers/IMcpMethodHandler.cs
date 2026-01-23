using System.Text.Json;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Handler for MCP JSON-RPC methods.
/// </summary>
public interface IMcpMethodHandler
{
    /// <summary>
    /// The method name this handler responds to (e.g., "tools/list", "tools/call").
    /// </summary>
    string Method { get; }

    /// <summary>
    /// Handles the MCP request and returns a result.
    /// </summary>
    Task<object?> HandleAsync(string playFrameworkName, JsonElement? parameters, CancellationToken cancellationToken = default);
}
