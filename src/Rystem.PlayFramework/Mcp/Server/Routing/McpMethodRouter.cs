namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Routes MCP JSON-RPC requests to the appropriate handler.
/// </summary>
public sealed class McpMethodRouter
{
    private readonly Dictionary<string, IMcpMethodHandler> _handlers;

    public McpMethodRouter(IEnumerable<IMcpMethodHandler> handlers)
    {
        _handlers = handlers.ToDictionary(h => h.Method, h => h);
    }

    /// <summary>
    /// Routes a request to the appropriate handler.
    /// </summary>
    public async Task<McpJsonRpcResponse> RouteAsync(
        string playFrameworkName,
        McpJsonRpcRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Method))
        {
            return McpJsonRpcResponse.Failure(
                request.Id,
                McpJsonRpcErrorCodes.InvalidRequest,
                "Method is required");
        }

        if (!_handlers.TryGetValue(request.Method, out var handler))
        {
            return McpJsonRpcResponse.Failure(
                request.Id,
                McpJsonRpcErrorCodes.MethodNotFound,
                $"Method '{request.Method}' not found");
        }

        try
        {
            var result = await handler.HandleAsync(playFrameworkName, request.Params, cancellationToken);
            return McpJsonRpcResponse.Success(request.Id, result);
        }
        catch (Exception ex)
        {
            return McpJsonRpcResponse.Failure(
                request.Id,
                McpJsonRpcErrorCodes.InternalError,
                ex.Message);
        }
    }

    /// <summary>
    /// Gets all supported methods.
    /// </summary>
    public IEnumerable<string> SupportedMethods => _handlers.Keys;
}
