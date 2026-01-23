using System.Text.RegularExpressions;

namespace Rystem.PlayFramework
{
    /// <summary>
    /// Implementation of resource filter configuration
    /// </summary>
    internal sealed class McpResourceFilterConfig : IMcpResourceFilterConfig
    {
        private Func<string, bool>? _predicate;

        public bool Enabled { get; set; } = true;

        public IMcpResourceFilterConfig WithResourcesMatching(string regexPattern)
        {
            var regex = new Regex(regexPattern);
            _predicate = uri => regex.IsMatch(uri);
            return this;
        }

        public IMcpResourceFilterConfig WithResourcesStartingWith(string prefix)
        {
            _predicate = uri => uri.StartsWith(prefix);
            return this;
        }

        public IMcpResourceFilterConfig WithResources(params string[] uris)
        {
            var uriSet = new HashSet<string>(uris);
            _predicate = uri => uriSet.Contains(uri);
            return this;
        }

        public IMcpResourceFilterConfig ExcludingResources(params string[] uris)
        {
            var uriSet = new HashSet<string>(uris);
            _predicate = uri => !uriSet.Contains(uri);
            return this;
        }

        public IMcpResourceFilterConfig WithResourcesPredicate(Func<string, bool> predicate)
        {
            _predicate = predicate;
            return this;
        }

        internal Func<string, bool>? GetPredicate() => _predicate;
    }
}
