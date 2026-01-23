using System.Text.RegularExpressions;

namespace Rystem.PlayFramework
{
    /// <summary>
    /// Implementation of tool filter configuration
    /// </summary>
    internal sealed class McpToolFilterConfig : IMcpToolFilterConfig
    {
        private Func<string, bool>? _predicate;

        public bool Enabled { get; set; } = true;

        public IMcpToolFilterConfig WithToolsMatching(string regexPattern)
        {
            var regex = new Regex(regexPattern);
            _predicate = name => regex.IsMatch(name);
            return this;
        }

        public IMcpToolFilterConfig WithToolsStartingWith(string prefix)
        {
            _predicate = name => name.StartsWith(prefix);
            return this;
        }

        public IMcpToolFilterConfig WithToolNames(params string[] names)
        {
            var nameSet = new HashSet<string>(names);
            _predicate = name => nameSet.Contains(name);
            return this;
        }

        public IMcpToolFilterConfig ExcludingTools(params string[] names)
        {
            var nameSet = new HashSet<string>(names);
            _predicate = name => !nameSet.Contains(name);
            return this;
        }

        public IMcpToolFilterConfig WithToolsPredicate(Func<string, bool> predicate)
        {
            _predicate = predicate;
            return this;
        }

        internal Func<string, bool>? GetPredicate() => _predicate;
    }
}
