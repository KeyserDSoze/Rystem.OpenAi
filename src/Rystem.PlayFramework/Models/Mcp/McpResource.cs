namespace Rystem.PlayFramework
{
    /// <summary>
    /// Represents a resource from an MCP server
    /// </summary>
    public sealed class McpResource
    {
        /// <summary>
        /// URI of the resource (e.g., "background-jobs://overview")
        /// </summary>
        public string Uri { get; set; } = string.Empty;

        /// <summary>
        /// Display name of the resource
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the resource content
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// MIME type of the content (e.g., "text/markdown")
        /// </summary>
        public string? MimeType { get; set; }
    }

    /// <summary>
    /// Content retrieved from an MCP resource
    /// </summary>
    public sealed class McpResourceContent
    {
        /// <summary>
        /// URI of the resource
        /// </summary>
        public string Uri { get; set; } = string.Empty;

        /// <summary>
        /// MIME type of the content
        /// </summary>
        public string? MimeType { get; set; }

        /// <summary>
        /// The actual content
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }
}
