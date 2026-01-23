using System.Text.Json;

namespace Rystem.PlayFramework
{
    /// <summary>
    /// Executes MCP tool calls
    /// </summary>
    internal sealed class McpToolExecutor : IMcpExecutor
    {
        public async Task<string> ExecuteToolAsync(McpToolCall mcpCall, string argumentsJson, CancellationToken cancellationToken)
        {
            try
            {
                // Parse arguments from JSON string
                var arguments = ParseArguments(argumentsJson);

                // Call the tool via MCP client
                var result = await mcpCall.Client.CallToolAsync(mcpCall.ToolName, arguments, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to execute MCP tool '{mcpCall.ToolName}' on server '{mcpCall.ServerName}': {ex.Message}",
                    ex);
            }
        }

        private Dictionary<string, object> ParseArguments(string json)
        {
            try
            {
                using var document = JsonDocument.Parse(json);
                var result = new Dictionary<string, object>();

                foreach (var property in document.RootElement.EnumerateObject())
                {
                    result[property.Name] = property.Value.ValueKind switch
                    {
                        JsonValueKind.Object or JsonValueKind.Array => property.Value.GetRawText(),
                        JsonValueKind.String => property.Value.GetString() ?? string.Empty,
                        JsonValueKind.Number => property.Value.GetRawText(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Null => null!,
                        _ => property.Value.GetRawText()
                    };
                }

                return result;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Invalid JSON in tool arguments: {ex.Message}", ex);
            }
        }
    }
}
