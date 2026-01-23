using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Rystem.PlayFramework;

namespace Rystem.PlayFramework.Mcp.Server;

/// <summary>
/// Handles tools/call MCP method.
/// Executes the PlayFramework's SceneManager.
/// </summary>
public sealed class ToolsCallHandler : IMcpMethodHandler
{
    private readonly IFactory<ISceneManager> _sceneManagerFactory;
    private readonly PlayFrameworkMcpServerRegistry _registry;

    public ToolsCallHandler(
        IFactory<ISceneManager> sceneManagerFactory,
        PlayFrameworkMcpServerRegistry registry)
    {
        _sceneManagerFactory = sceneManagerFactory;
        _registry = registry;
    }

    public string Method => "tools/call";

    public async Task<object?> HandleAsync(string playFrameworkName, JsonElement? parameters, CancellationToken cancellationToken = default)
    {
        var info = _registry.Get(playFrameworkName);
        if (info == null)
        {
            return new McpToolCallResponse
            {
                Content = [new McpServerToolResultContent { Text = $"PlayFramework '{playFrameworkName}' not found" }],
                IsError = true
            };
        }

        // Extract the message from parameters
        string? message = null;
        if (parameters.HasValue)
        {
            var paramsObj = parameters.Value;
            if (paramsObj.TryGetProperty("arguments", out var args) &&
                args.TryGetProperty("message", out var msgElement))
            {
                message = msgElement.GetString();
            }
            else if (paramsObj.TryGetProperty("message", out var directMsg))
            {
                message = directMsg.GetString();
            }
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            return new McpToolCallResponse
            {
                Content = [new McpServerToolResultContent { Text = "Missing required parameter: message" }],
                IsError = true
            };
        }

        try
        {
            // Get the SceneManager for this PlayFramework
            var sceneManager = _sceneManagerFactory.Create(playFrameworkName);
            if (sceneManager == null)
            {
                return new McpToolCallResponse
                {
                    Content = [new McpServerToolResultContent { Text = $"Could not create SceneManager for '{playFrameworkName}'" }],
                    IsError = true
                };
            }

            // Execute the request - consume the IAsyncEnumerable and get the final response
            AiSceneResponse? lastResponse = null;
            await foreach (var response in sceneManager.ExecuteAsync(message, cancellationToken: cancellationToken))
            {
                lastResponse = response;
            }

            return new McpToolCallResponse
            {
                Content = [new McpServerToolResultContent { Text = lastResponse?.Response ?? "No response generated" }],
                IsError = false
            };
        }
        catch (Exception ex)
        {
            return new McpToolCallResponse
            {
                Content = [new McpServerToolResultContent { Text = $"Error executing PlayFramework: {ex.Message}" }],
                IsError = true
            };
        }
    }
}
