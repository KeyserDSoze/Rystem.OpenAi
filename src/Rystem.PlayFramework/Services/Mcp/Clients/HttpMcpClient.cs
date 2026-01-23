using System.Text.Json;
using Rystem.OpenAi;

namespace Rystem.PlayFramework
{
    /// <summary>
    /// HTTP-based MCP client for communicating with MCP servers via HTTP
    /// </summary>
    internal sealed class HttpMcpClient : IMcpClient
    {
        private readonly string _serverUrl;
        private readonly IHttpClientFactory _httpClientFactory;
        private bool _isConnected;

        public bool IsConnected => _isConnected;

        public HttpMcpClient(string serverUrl, IHttpClientFactory httpClientFactory)
        {
            _serverUrl = serverUrl.TrimEnd('/');
            _httpClientFactory = httpClientFactory;
            _isConnected = false;
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            if (_isConnected)
                return;

            try
            {
                using var client = _httpClientFactory.CreateClient();
                // Simple health check by calling tools/list
                var response = await SendJsonRpcAsync<object>("tools/list", null, cancellationToken);
                _isConnected = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to connect to MCP server at {_serverUrl}", ex);
            }
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            _isConnected = false;
            await Task.CompletedTask;
        }

        public async Task<IReadOnlyList<FunctionTool>> ListToolsAsync(CancellationToken cancellationToken)
        {
            if (!_isConnected)
                await ConnectAsync(cancellationToken);

            var response = await SendJsonRpcAsync<ToolsListResponse>("tools/list", null, cancellationToken);

            if (response?.Tools == null || response.Tools.Count == 0)
                return Array.Empty<FunctionTool>();

            var tools = new List<FunctionTool>();

            foreach (var toolData in response.Tools)
            {
                try
                {
                    // Deserialize inputSchema directly to FunctionToolMainProperty
                    var parameters = toolData.InputSchema != null
                        ? JsonSerializer.Deserialize<FunctionToolMainProperty>(
                            JsonSerializer.Serialize(toolData.InputSchema))
                        : new FunctionToolMainProperty();

                    var functionTool = new FunctionTool
                    {
                        Name = toolData.Name,
                        Description = toolData.Description ?? string.Empty,
                        Parameters = parameters ?? new FunctionToolMainProperty()
                    };

                    tools.Add(functionTool);
                }
                catch (Exception ex)
                {
                    // Log but continue with other tools
                    System.Diagnostics.Debug.WriteLine($"Failed to parse tool {toolData.Name}: {ex.Message}");
                }
            }

            return tools.AsReadOnly();
        }

        public async Task<string> CallToolAsync(string toolName, Dictionary<string, object> arguments, CancellationToken cancellationToken)
        {
            if (!_isConnected)
                await ConnectAsync(cancellationToken);

            var toolCallParams = new
            {
                name = toolName,
                arguments = arguments
            };

            var response = await SendJsonRpcAsync<ToolCallResponse>("tools/call", toolCallParams, cancellationToken);

            if (response?.Content == null || response.Content.Count == 0)
                return string.Empty;

            // Combine all content pieces into a single result
            var result = string.Join("\n", response.Content.Select(c => c.Text ?? string.Empty));
            return result;
        }

        // === RESOURCES ===

        public async Task<IReadOnlyList<McpResource>> ListResourcesAsync(CancellationToken cancellationToken)
        {
            if (!_isConnected)
                await ConnectAsync(cancellationToken);

            try
            {
                var response = await SendJsonRpcAsync<ResourcesListResponse>("resources/list", null, cancellationToken);

                if (response?.Resources == null || response.Resources.Count == 0)
                    return Array.Empty<McpResource>();

                return response.Resources.Select(r => new McpResource
                {
                    Uri = r.Uri,
                    Name = r.Name,
                    Description = r.Description,
                    MimeType = r.MimeType
                }).ToList().AsReadOnly();
            }
            catch
            {
                // Resources not supported, return empty
                return Array.Empty<McpResource>();
            }
        }

        public async Task<McpResourceContent> ReadResourceAsync(string uri, CancellationToken cancellationToken)
        {
            if (!_isConnected)
                await ConnectAsync(cancellationToken);

            var response = await SendJsonRpcAsync<ResourceReadResponse>("resources/read", new { uri }, cancellationToken);

            if (response?.Contents == null || response.Contents.Count == 0)
                return new McpResourceContent { Uri = uri, Content = string.Empty };

            var content = response.Contents.FirstOrDefault();
            return new McpResourceContent
            {
                Uri = uri,
                MimeType = content?.MimeType,
                Content = content?.Text ?? string.Empty
            };
        }

        // === PROMPTS ===

        public async Task<IReadOnlyList<McpPrompt>> ListPromptsAsync(CancellationToken cancellationToken)
        {
            if (!_isConnected)
                await ConnectAsync(cancellationToken);

            try
            {
                var response = await SendJsonRpcAsync<PromptsListResponse>("prompts/list", null, cancellationToken);

                if (response?.Prompts == null || response.Prompts.Count == 0)
                    return Array.Empty<McpPrompt>();

                return response.Prompts.Select(p => new McpPrompt
                {
                    Name = p.Name,
                    Description = p.Description,
                    Arguments = p.Arguments?.Select(a => new McpPromptArgument
                    {
                        Name = a.Name,
                        Description = a.Description,
                        Required = a.Required
                    }).ToList()
                }).ToList().AsReadOnly();
            }
            catch
            {
                // Prompts not supported, return empty
                return Array.Empty<McpPrompt>();
            }
        }

        public async Task<McpPromptContent> GetPromptAsync(string promptName, Dictionary<string, string>? arguments, CancellationToken cancellationToken)
        {
            if (!_isConnected)
                await ConnectAsync(cancellationToken);

            var response = await SendJsonRpcAsync<PromptGetResponse>("prompts/get", new { name = promptName, arguments }, cancellationToken);

            return new McpPromptContent
            {
                PromptName = promptName,
                Description = response?.Description,
                Content = response?.Messages?.Select(m => new McpPromptContentBlock
                {
                    Type = m.Content?.Type ?? "text",
                    Text = m.Content?.Text,
                    ResourceUri = m.Content?.Resource
                }).ToList() ?? []
            };
        }

        private async Task<T> SendJsonRpcAsync<T>(string method, object? @params, CancellationToken cancellationToken) where T : class
        {
            using var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            var jsonRpcRequest = new
            {
                jsonrpc = "2.0",
                method = method,
                @params = @params,
                id = Guid.NewGuid().ToString()
            };

            var content = new StringContent(
                JsonSerializer.Serialize(jsonRpcRequest),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync($"{_serverUrl}/mcp", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

            if (jsonResponse.ValueKind != System.Text.Json.JsonValueKind.Object)
                throw new InvalidOperationException("Invalid JSON-RPC response");

            if (jsonResponse.TryGetProperty("error", out var errorElement) && errorElement.ValueKind != System.Text.Json.JsonValueKind.Null)
            {
                var errorMessage = errorElement.TryGetProperty("message", out var msg)
                    ? msg.GetString() ?? "Unknown error"
                    : "Unknown JSON-RPC error";
                throw new InvalidOperationException($"MCP server error: {errorMessage}");
            }

            if (!jsonResponse.TryGetProperty("result", out var resultElement))
                throw new InvalidOperationException("No result in JSON-RPC response");

            return JsonSerializer.Deserialize<T>(resultElement.GetRawText());
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync(CancellationToken.None);
        }

        // Response DTOs for JSON deserialization

        // === TOOLS DTOs ===
        private class ToolsListResponse
        {
            public List<ToolData> Tools { get; set; } = [];
        }

        private class ToolData
        {
            [System.Text.Json.Serialization.JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("description")]
            public string? Description { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("inputSchema")]
            public object? InputSchema { get; set; }
        }

        private class ToolCallResponse
        {
            public List<ContentPiece> Content { get; set; } = [];
        }

        private class ContentPiece
        {
            [System.Text.Json.Serialization.JsonPropertyName("type")]
            public string Type { get; set; } = "text";

            [System.Text.Json.Serialization.JsonPropertyName("text")]
            public string? Text { get; set; }
        }

        // === RESOURCES DTOs ===
        private class ResourcesListResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("resources")]
            public List<ResourceData> Resources { get; set; } = [];
        }

        private class ResourceData
        {
            [System.Text.Json.Serialization.JsonPropertyName("uri")]
            public string Uri { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("description")]
            public string? Description { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("mimeType")]
            public string? MimeType { get; set; }
        }

        private class ResourceReadResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("contents")]
            public List<ResourceContent> Contents { get; set; } = [];
        }

        private class ResourceContent
        {
            [System.Text.Json.Serialization.JsonPropertyName("uri")]
            public string Uri { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("mimeType")]
            public string? MimeType { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("text")]
            public string? Text { get; set; }
        }

        // === PROMPTS DTOs ===
        private class PromptsListResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("prompts")]
            public List<PromptData> Prompts { get; set; } = [];
        }

        private class PromptData
        {
            [System.Text.Json.Serialization.JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("description")]
            public string? Description { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("arguments")]
            public List<PromptArgumentData>? Arguments { get; set; }
        }

        private class PromptArgumentData
        {
            [System.Text.Json.Serialization.JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("description")]
            public string? Description { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("required")]
            public bool Required { get; set; }
        }

        private class PromptGetResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("description")]
            public string? Description { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("messages")]
            public List<PromptMessage>? Messages { get; set; }
        }

        private class PromptMessage
        {
            [System.Text.Json.Serialization.JsonPropertyName("role")]
            public string Role { get; set; } = "user";

            [System.Text.Json.Serialization.JsonPropertyName("content")]
            public PromptMessageContent? Content { get; set; }
        }

        private class PromptMessageContent
        {
            [System.Text.Json.Serialization.JsonPropertyName("type")]
            public string Type { get; set; } = "text";

            [System.Text.Json.Serialization.JsonPropertyName("text")]
            public string? Text { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("resource")]
            public string? Resource { get; set; }
        }
    }
}
