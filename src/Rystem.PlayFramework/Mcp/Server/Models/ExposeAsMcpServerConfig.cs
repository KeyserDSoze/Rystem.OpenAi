namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Configuration for exposing a PlayFramework as an MCP server.
/// </summary>
public sealed class ExposeAsMcpServerConfig
{
    /// <summary>
    /// Description of the PlayFramework shown to MCP clients.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional system prompt to expose via prompts/list.
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// Whether to enable resources (scene documentation). Default: true.
    /// </summary>
    public bool EnableResources { get; set; } = true;

    /// <summary>
    /// The .NET authorization policy name to apply. Null means public access.
    /// </summary>
    public string? AuthorizationPolicy { get; set; }
}
