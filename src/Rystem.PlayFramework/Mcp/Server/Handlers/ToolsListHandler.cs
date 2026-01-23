using System.Text.Json;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Handles tools/list MCP method.
/// Returns the PlayFramework as a single tool.
/// </summary>
public sealed class ToolsListHandler : IMcpMethodHandler
{
    private readonly PlayFrameworkMcpServerRegistry _registry;

    public ToolsListHandler(PlayFrameworkMcpServerRegistry registry)
    {
        _registry = registry;
    }

    public string Method => "tools/list";

    public Task<object?> HandleAsync(string playFrameworkName, JsonElement? parameters, CancellationToken cancellationToken = default)
    {
        var info = _registry.Get(playFrameworkName);
        if (info == null)
        {
            return Task.FromResult<object?>(new McpListResponse<McpServerToolInfo> { Tools = [] });
        }

        var tool = new McpServerToolInfo
        {
            Name = info.Name,
            Description = info.Description ?? $"AI assistant powered by {info.Name} PlayFramework",
            InputSchema = new McpServerToolInputSchema
            {
                Type = "object",
                Properties = new Dictionary<string, McpServerToolProperty>
                {
                    ["message"] = new McpServerToolProperty
                    {
                        Type = "string",
                        Description = "The message or request to send to the AI assistant"
                    }
                },
                Required = ["message"]
            }
        };

        return Task.FromResult<object?>(new McpListResponse<McpServerToolInfo> { Tools = [tool] });
    }
}
