using System.Text.Json.Serialization;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Tool information for tools/list response.
/// </summary>
public sealed class McpServerToolInfo
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("inputSchema")]
    public required McpServerToolInputSchema InputSchema { get; init; }
}

/// <summary>
/// Input schema for a tool.
/// </summary>
public sealed class McpServerToolInputSchema
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "object";

    [JsonPropertyName("properties")]
    public Dictionary<string, McpServerToolProperty> Properties { get; init; } = new();

    [JsonPropertyName("required")]
    public List<string> Required { get; init; } = [];
}

/// <summary>
/// Property definition for tool input.
/// </summary>
public sealed class McpServerToolProperty
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "string";

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

/// <summary>
/// Resource information for resources/list response.
/// </summary>
public sealed class McpServerResourceInfo
{
    [JsonPropertyName("uri")]
    public required string Uri { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("mimeType")]
    public string MimeType { get; init; } = "text/markdown";
}

/// <summary>
/// Resource content for resources/read response.
/// </summary>
public sealed class McpServerResourceContent
{
    [JsonPropertyName("uri")]
    public required string Uri { get; init; }

    [JsonPropertyName("mimeType")]
    public string MimeType { get; init; } = "text/markdown";

    [JsonPropertyName("text")]
    public required string Text { get; init; }
}

/// <summary>
/// Prompt information for prompts/list response.
/// </summary>
public sealed class McpServerPromptInfo
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("arguments")]
    public List<McpServerPromptArgument>? Arguments { get; init; }
}

/// <summary>
/// Prompt argument definition.
/// </summary>
public sealed class McpServerPromptArgument
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("required")]
    public bool Required { get; init; }
}

/// <summary>
/// Prompt message for prompts/get response.
/// </summary>
public sealed class McpServerPromptMessage
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = "user";

    [JsonPropertyName("content")]
    public required McpServerPromptContent Content { get; init; }
}

/// <summary>
/// Content of a prompt message.
/// </summary>
public sealed class McpServerPromptContent
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "text";

    [JsonPropertyName("text")]
    public required string Text { get; init; }
}

/// <summary>
/// Tool call result content.
/// </summary>
public sealed class McpServerToolResultContent
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "text";

    [JsonPropertyName("text")]
    public required string Text { get; init; }
}

/// <summary>
/// Response wrapper for lists.
/// </summary>
public sealed class McpListResponse<T>
{
    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<T>? Tools { get; init; }

    [JsonPropertyName("resources")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<T>? Resources { get; init; }

    [JsonPropertyName("prompts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<T>? Prompts { get; init; }
}

/// <summary>
/// Response for tools/call.
/// </summary>
public sealed class McpToolCallResponse
{
    [JsonPropertyName("content")]
    public required List<McpServerToolResultContent> Content { get; init; }

    [JsonPropertyName("isError")]
    public bool IsError { get; init; }
}

/// <summary>
/// Response for resources/read.
/// </summary>
public sealed class McpResourceReadResponse
{
    [JsonPropertyName("contents")]
    public required List<McpServerResourceContent> Contents { get; init; }
}

/// <summary>
/// Response for prompts/get.
/// </summary>
public sealed class McpPromptGetResponse
{
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("messages")]
    public required List<McpServerPromptMessage> Messages { get; init; }
}
