namespace Rystem.PlayFramework
{
    /// <summary>
    /// Represents a prompt template from an MCP server
    /// </summary>
    public sealed class McpPrompt
    {
        /// <summary>
        /// Name of the prompt
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the prompt
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Arguments that the prompt accepts
        /// </summary>
        public List<McpPromptArgument>? Arguments { get; set; }
    }

    /// <summary>
    /// Argument definition for an MCP prompt
    /// </summary>
    public sealed class McpPromptArgument
    {
        /// <summary>
        /// Name of the argument
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the argument
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Whether the argument is required
        /// </summary>
        public bool Required { get; set; }
    }

    /// <summary>
    /// Content retrieved from an MCP prompt
    /// </summary>
    public sealed class McpPromptContent
    {
        /// <summary>
        /// Name of the prompt
        /// </summary>
        public string PromptName { get; set; } = string.Empty;

        /// <summary>
        /// Description of the prompt
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Content blocks that make up the prompt
        /// </summary>
        public List<McpPromptContentBlock> Content { get; set; } = [];
    }

    /// <summary>
    /// A single content block in a prompt
    /// </summary>
    public sealed class McpPromptContentBlock
    {
        /// <summary>
        /// Type of content ("text" or "resource")
        /// </summary>
        public string Type { get; set; } = "text";

        /// <summary>
        /// Text content (if type is "text")
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Resource URI (if type is "resource")
        /// </summary>
        public string? ResourceUri { get; set; }
    }
}
