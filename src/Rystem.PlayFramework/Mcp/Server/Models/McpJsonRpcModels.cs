using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// JSON-RPC 2.0 request for MCP protocol.
/// </summary>
public sealed class McpJsonRpcRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public object? Id { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("params")]
    public JsonElement? Params { get; set; }
}

/// <summary>
/// JSON-RPC 2.0 response for MCP protocol.
/// </summary>
public sealed class McpJsonRpcResponse
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public object? Id { get; set; }

    [JsonPropertyName("result")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Result { get; set; }

    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public McpJsonRpcError? Error { get; set; }

    public static McpJsonRpcResponse Success(object? id, object? result) => new()
    {
        Id = id,
        Result = result
    };

    public static McpJsonRpcResponse Failure(object? id, int code, string message) => new()
    {
        Id = id,
        Error = new McpJsonRpcError { Code = code, Message = message }
    };
}

/// <summary>
/// JSON-RPC 2.0 error object.
/// </summary>
public sealed class McpJsonRpcError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; set; }
}

/// <summary>
/// Standard JSON-RPC error codes.
/// </summary>
public static class McpJsonRpcErrorCodes
{
    public const int ParseError = -32700;
    public const int InvalidRequest = -32600;
    public const int MethodNotFound = -32601;
    public const int InvalidParams = -32602;
    public const int InternalError = -32603;
}
