namespace Rystem.PlayFramework
{
    /// <summary>
    /// Configuration interface for MCP resource filtering
    /// </summary>
    public interface IMcpResourceFilterConfig
    {
        /// <summary>
        /// Whether resources are enabled
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Only resources matching this regex pattern are available
        /// </summary>
        IMcpResourceFilterConfig WithResourcesMatching(string regexPattern);

        /// <summary>
        /// Only resources starting with this prefix are available
        /// </summary>
        IMcpResourceFilterConfig WithResourcesStartingWith(string prefix);

        /// <summary>
        /// Only these specific resource URIs are available
        /// </summary>
        IMcpResourceFilterConfig WithResources(params string[] uris);

        /// <summary>
        /// All resources EXCEPT these are available
        /// </summary>
        IMcpResourceFilterConfig ExcludingResources(params string[] uris);

        /// <summary>
        /// Custom predicate for filtering
        /// </summary>
        IMcpResourceFilterConfig WithResourcesPredicate(Func<string, bool> predicate);
    }
}
