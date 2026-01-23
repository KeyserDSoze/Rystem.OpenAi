namespace Rystem.PlayFramework
{
    /// <summary>
    /// Fluent builder for configuring MCP element filters
    /// </summary>
    internal sealed class McpServerToolFilterBuilder : IMcpServerToolFilterBuilder
    {
        private McpToolFilterConfig? _toolConfig;
        private McpResourceFilterConfig? _resourceConfig;
        private McpPromptFilterConfig? _promptConfig;

        // === TOOLS ===

        public IMcpServerToolFilterBuilder WithTools(Action<IMcpToolFilterConfig>? builder = null)
        {
            _toolConfig = new McpToolFilterConfig();
            builder?.Invoke(_toolConfig);
            return this;
        }

        public IMcpServerToolFilterBuilder OnlyTools(Action<IMcpToolFilterConfig>? builder = null)
        {
            WithTools(builder);
            _resourceConfig = new McpResourceFilterConfig { Enabled = false };
            _promptConfig = new McpPromptFilterConfig { Enabled = false };
            return this;
        }

        // === RESOURCES ===

        public IMcpServerToolFilterBuilder WithResources(Action<IMcpResourceFilterConfig>? builder = null)
        {
            _resourceConfig = new McpResourceFilterConfig();
            builder?.Invoke(_resourceConfig);
            return this;
        }

        public IMcpServerToolFilterBuilder OnlyResources(Action<IMcpResourceFilterConfig>? builder = null)
        {
            WithResources(builder);
            _toolConfig = new McpToolFilterConfig { Enabled = false };
            _promptConfig = new McpPromptFilterConfig { Enabled = false };
            return this;
        }

        // === PROMPTS ===

        public IMcpServerToolFilterBuilder WithPrompts(Action<IMcpPromptFilterConfig>? builder = null)
        {
            _promptConfig = new McpPromptFilterConfig();
            builder?.Invoke(_promptConfig);
            return this;
        }

        public IMcpServerToolFilterBuilder OnlyPrompts(Action<IMcpPromptFilterConfig>? builder = null)
        {
            WithPrompts(builder);
            _toolConfig = new McpToolFilterConfig { Enabled = false };
            _resourceConfig = new McpResourceFilterConfig { Enabled = false };
            return this;
        }

        public McpSceneFilter Build()
        {
            return new McpSceneFilter
            {
                ToolsEnabled = _toolConfig?.Enabled ?? true,
                ToolPredicate = _toolConfig?.GetPredicate(),

                ResourcesEnabled = _resourceConfig?.Enabled ?? true,
                ResourcePredicate = _resourceConfig?.GetPredicate(),

                PromptsEnabled = _promptConfig?.Enabled ?? true,
                PromptPredicate = _promptConfig?.GetPredicate()
            };
        }
    }
}
