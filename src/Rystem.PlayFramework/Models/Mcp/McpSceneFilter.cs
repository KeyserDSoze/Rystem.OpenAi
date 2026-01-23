namespace Rystem.PlayFramework
{
    /// <summary>
    /// Complete filter configuration for MCP elements in a scene
    /// </summary>
    public sealed class McpSceneFilter
    {
        /// <summary>
        /// Whether tools are enabled for this scene
        /// </summary>
        public bool ToolsEnabled { get; set; } = true;

        /// <summary>
        /// Predicate to filter which tools are available
        /// </summary>
        public Func<string, bool>? ToolPredicate { get; set; }

        /// <summary>
        /// Whether resources are enabled for this scene
        /// </summary>
        public bool ResourcesEnabled { get; set; } = true;

        /// <summary>
        /// Predicate to filter which resources are available
        /// </summary>
        public Func<string, bool>? ResourcePredicate { get; set; }

        /// <summary>
        /// Whether prompts are enabled for this scene
        /// </summary>
        public bool PromptsEnabled { get; set; } = true;

        /// <summary>
        /// Predicate to filter which prompts are available
        /// </summary>
        public Func<string, bool>? PromptPredicate { get; set; }

        /// <summary>
        /// Check if a tool matches the filter
        /// </summary>
        public bool MatchesTool(string toolName) 
            => ToolsEnabled && (ToolPredicate?.Invoke(toolName) ?? true);

        /// <summary>
        /// Check if a resource matches the filter
        /// </summary>
        public bool MatchesResource(string resourceUri) 
            => ResourcesEnabled && (ResourcePredicate?.Invoke(resourceUri) ?? true);

        /// <summary>
        /// Check if a prompt matches the filter
        /// </summary>
        public bool MatchesPrompt(string promptName) 
            => PromptsEnabled && (PromptPredicate?.Invoke(promptName) ?? true);
    }
}
