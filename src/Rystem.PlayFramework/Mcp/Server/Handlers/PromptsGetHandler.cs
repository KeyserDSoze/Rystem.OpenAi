using System.Text.Json;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Handles prompts/get MCP method.
/// Returns the content of a specific prompt.
/// </summary>
public sealed class PromptsGetHandler : IMcpMethodHandler
{
    private readonly PlayFrameworkMcpServerRegistry _registry;

    public PromptsGetHandler(PlayFrameworkMcpServerRegistry registry)
    {
        _registry = registry;
    }

    public string Method => "prompts/get";

    public Task<object?> HandleAsync(string playFrameworkName, JsonElement? parameters, CancellationToken cancellationToken = default)
    {
        var info = _registry.Get(playFrameworkName);
        if (info == null || string.IsNullOrWhiteSpace(info.Prompt))
        {
            return Task.FromResult<object?>(new McpPromptGetResponse
            {
                Messages = []
            });
        }

        // Extract optional context parameter
        string? context = null;
        if (parameters.HasValue &&
            parameters.Value.TryGetProperty("arguments", out var args) &&
            args.TryGetProperty("context", out var contextElement))
        {
            context = contextElement.GetString();
        }

        // Build the prompt message
        var promptText = info.Prompt;
        if (!string.IsNullOrWhiteSpace(context))
        {
            promptText = $"{info.Prompt}\n\nAdditional Context:\n{context}";
        }

        return Task.FromResult<object?>(new McpPromptGetResponse
        {
            Description = info.Description,
            Messages =
            [
                new McpServerPromptMessage
                {
                    Role = "user",
                    Content = new McpServerPromptContent
                    {
                        Type = "text",
                        Text = promptText
                    }
                }
            ]
        });
    }
}
