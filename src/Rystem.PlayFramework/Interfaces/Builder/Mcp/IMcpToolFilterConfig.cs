namespace Rystem.PlayFramework
{
    /// <summary>
    /// Configuration interface for MCP tool filtering
    /// </summary>
    public interface IMcpToolFilterConfig
    {
        /// <summary>
        /// Whether tools are enabled
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Only tools matching this regex pattern are available
        /// </summary>
        IMcpToolFilterConfig WithToolsMatching(string regexPattern);

        /// <summary>
        /// Only tools starting with this prefix are available
        /// </summary>
        IMcpToolFilterConfig WithToolsStartingWith(string prefix);

        /// <summary>
        /// Only these specific tool names are available
        /// </summary>
        IMcpToolFilterConfig WithToolNames(params string[] names);

        /// <summary>
        /// All tools EXCEPT these are available
        /// </summary>
        IMcpToolFilterConfig ExcludingTools(params string[] names);

        /// <summary>
        /// Custom predicate for filtering
        /// </summary>
        IMcpToolFilterConfig WithToolsPredicate(Func<string, bool> predicate);
    }
}
