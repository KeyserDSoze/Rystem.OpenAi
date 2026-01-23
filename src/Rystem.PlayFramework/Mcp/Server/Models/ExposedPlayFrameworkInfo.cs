namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Information about a PlayFramework exposed as an MCP server.
/// </summary>
public sealed class ExposedPlayFrameworkInfo
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? Prompt { get; init; }
    public bool EnableResources { get; init; } = true;
    public string? AuthorizationPolicy { get; init; }
    public List<SceneDocumentation> SceneDocumentations { get; init; } = [];
}

/// <summary>
/// Documentation for a single scene, used as MCP resource.
/// </summary>
public sealed class SceneDocumentation
{
    public required string SceneName { get; init; }
    public string? Description { get; init; }
    public string? SystemMessage { get; init; }
    public List<string> AvailableTools { get; init; } = [];
    public List<string> AvailableActors { get; init; } = [];
}
