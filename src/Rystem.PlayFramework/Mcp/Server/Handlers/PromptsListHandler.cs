using System.Text.Json;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Handles prompts/list MCP method.
/// Returns prompts configured for the PlayFramework.
/// </summary>
public sealed class PromptsListHandler : IMcpMethodHandler
{
    private readonly PlayFrameworkMcpServerRegistry _registry;

    public PromptsListHandler(PlayFrameworkMcpServerRegistry registry)
    {
        _registry = registry;
    }

    public string Method => "prompts/list";

    public Task<object?> HandleAsync(string playFrameworkName, JsonElement? parameters, CancellationToken cancellationToken = default)
    {
        var info = _registry.Get(playFrameworkName);
        if (info == null || string.IsNullOrWhiteSpace(info.Prompt))
        {
            return Task.FromResult<object?>(new McpListResponse<McpServerPromptInfo> { Prompts = [] });
        }

        var prompts = new List<McpServerPromptInfo>
        {
            new McpServerPromptInfo
            {
                Name = $"{info.Name}-system-prompt",
                Description = info.Description ?? $"System prompt for {info.Name}",
                Arguments =
                [
                    new McpServerPromptArgument
                    {
                        Name = "context",
                        Description = "Optional additional context to include",
                        Required = false
                    }
                ]
            }
        };

        return Task.FromResult<object?>(new McpListResponse<McpServerPromptInfo> { Prompts = prompts });
    }
}
