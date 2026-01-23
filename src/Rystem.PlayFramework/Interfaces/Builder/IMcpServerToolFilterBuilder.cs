namespace Rystem.PlayFramework
{
    /// <summary>
    /// Fluent builder for configuring which MCP elements are available in a scene
    /// </summary>
    public interface IMcpServerToolFilterBuilder
    {
        // === TOOLS ===

        /// <summary>
        /// Configure tools with optional filter. Keeps other elements enabled.
        /// </summary>
        IMcpServerToolFilterBuilder WithTools(Action<IMcpToolFilterConfig>? builder = null);

        /// <summary>
        /// Use ONLY tools (disables resources and prompts)
        /// </summary>
        IMcpServerToolFilterBuilder OnlyTools(Action<IMcpToolFilterConfig>? builder = null);

        // === RESOURCES ===

        /// <summary>
        /// Configure resources with optional filter. Keeps other elements enabled.
        /// </summary>
        IMcpServerToolFilterBuilder WithResources(Action<IMcpResourceFilterConfig>? builder = null);

        /// <summary>
        /// Use ONLY resources (disables tools and prompts)
        /// </summary>
        IMcpServerToolFilterBuilder OnlyResources(Action<IMcpResourceFilterConfig>? builder = null);

        // === PROMPTS ===

        /// <summary>
        /// Configure prompts with optional filter. Keeps other elements enabled.
        /// </summary>
        IMcpServerToolFilterBuilder WithPrompts(Action<IMcpPromptFilterConfig>? builder = null);

        /// <summary>
        /// Use ONLY prompts (disables tools and resources)
        /// </summary>
        IMcpServerToolFilterBuilder OnlyPrompts(Action<IMcpPromptFilterConfig>? builder = null);

        /// <summary>
        /// Internal method to build the filter
        /// </summary>
        internal McpSceneFilter Build();
    }
}
