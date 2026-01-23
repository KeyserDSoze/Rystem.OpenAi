using System.Text.Json;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Handles resources/read MCP method.
/// Returns the content of documentation resources.
/// </summary>
public sealed class ResourcesReadHandler : IMcpMethodHandler
{
    private readonly PlayFrameworkMcpServerRegistry _registry;
    private readonly PlayFrameworkDocumentationBuilder _docBuilder;

    public ResourcesReadHandler(
        PlayFrameworkMcpServerRegistry registry,
        PlayFrameworkDocumentationBuilder docBuilder)
    {
        _registry = registry;
        _docBuilder = docBuilder;
    }

    public string Method => "resources/read";

    public Task<object?> HandleAsync(string playFrameworkName, JsonElement? parameters, CancellationToken cancellationToken = default)
    {
        var info = _registry.Get(playFrameworkName);
        if (info == null || !info.EnableResources)
        {
            return Task.FromResult<object?>(new McpResourceReadResponse { Contents = [] });
        }

        // Extract URI from parameters
        string? uri = null;
        if (parameters.HasValue && parameters.Value.TryGetProperty("uri", out var uriElement))
        {
            uri = uriElement.GetString();
        }

        if (string.IsNullOrWhiteSpace(uri))
        {
            return Task.FromResult<object?>(new McpResourceReadResponse { Contents = [] });
        }

        // Parse the URI to determine what to return
        // Format: playframework://{name}/overview or playframework://{name}/scenes/{sceneName}
        var content = GetResourceContent(info, uri);
        if (content == null)
        {
            return Task.FromResult<object?>(new McpResourceReadResponse { Contents = [] });
        }

        return Task.FromResult<object?>(new McpResourceReadResponse
        {
            Contents =
            [
                new McpServerResourceContent
                {
                    Uri = uri,
                    MimeType = "text/markdown",
                    Text = content
                }
            ]
        });
    }

    private string? GetResourceContent(ExposedPlayFrameworkInfo info, string uri)
    {
        var prefix = $"playframework://{info.Name}/";
        if (!uri.StartsWith(prefix))
        {
            return null;
        }

        var path = uri[prefix.Length..];

        if (path == "overview")
        {
            return _docBuilder.BuildOverviewDocumentation(info);
        }

        if (path.StartsWith("scenes/"))
        {
            var sceneName = path["scenes/".Length..];
            var scene = info.SceneDocumentations.FirstOrDefault(s =>
                s.SceneName.Equals(sceneName, StringComparison.OrdinalIgnoreCase));

            if (scene != null)
            {
                return _docBuilder.BuildSceneDocumentation(scene);
            }
        }

        return null;
    }
}
