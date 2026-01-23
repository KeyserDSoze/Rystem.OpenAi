using System.Text.Json;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Handles resources/list MCP method.
/// Returns documentation resources for the PlayFramework scenes.
/// </summary>
public sealed class ResourcesListHandler : IMcpMethodHandler
{
    private readonly PlayFrameworkMcpServerRegistry _registry;

    public ResourcesListHandler(PlayFrameworkMcpServerRegistry registry)
    {
        _registry = registry;
    }

    public string Method => "resources/list";

    public Task<object?> HandleAsync(string playFrameworkName, JsonElement? parameters, CancellationToken cancellationToken = default)
    {
        var info = _registry.Get(playFrameworkName);
        if (info == null || !info.EnableResources)
        {
            return Task.FromResult<object?>(new McpListResponse<McpServerResourceInfo> { Resources = [] });
        }

        var resources = new List<McpServerResourceInfo>
        {
            // Overview resource
            new McpServerResourceInfo
            {
                Uri = $"playframework://{playFrameworkName}/overview",
                Name = $"{playFrameworkName} Overview",
                Description = $"Complete documentation for {playFrameworkName} PlayFramework",
                MimeType = "text/markdown"
            }
        };

        // Add each scene as a resource
        foreach (var scene in info.SceneDocumentations)
        {
            resources.Add(new McpServerResourceInfo
            {
                Uri = $"playframework://{playFrameworkName}/scenes/{scene.SceneName}",
                Name = scene.SceneName,
                Description = scene.Description ?? $"Documentation for {scene.SceneName} scene",
                MimeType = "text/markdown"
            });
        }

        return Task.FromResult<object?>(new McpListResponse<McpServerResourceInfo> { Resources = resources });
    }
}
