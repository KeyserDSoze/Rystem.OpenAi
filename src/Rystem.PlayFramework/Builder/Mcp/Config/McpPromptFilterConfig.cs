using System.Text.RegularExpressions;

namespace Rystem.PlayFramework
{
    /// <summary>
    /// Implementation of prompt filter configuration
    /// </summary>
    internal sealed class McpPromptFilterConfig : IMcpPromptFilterConfig
    {
        private Func<string, bool>? _predicate;

        public bool Enabled { get; set; } = true;

        public IMcpPromptFilterConfig WithPromptsMatching(string regexPattern)
        {
            var regex = new Regex(regexPattern);
            _predicate = name => regex.IsMatch(name);
            return this;
        }

        public IMcpPromptFilterConfig WithPromptsStartingWith(string prefix)
        {
            _predicate = name => name.StartsWith(prefix);
            return this;
        }

        public IMcpPromptFilterConfig WithPrompts(params string[] names)
        {
            var nameSet = new HashSet<string>(names);
            _predicate = name => nameSet.Contains(name);
            return this;
        }

        public IMcpPromptFilterConfig ExcludingPrompts(params string[] names)
        {
            var nameSet = new HashSet<string>(names);
            _predicate = name => !nameSet.Contains(name);
            return this;
        }

        public IMcpPromptFilterConfig WithPromptsPredicate(Func<string, bool> predicate)
        {
            _predicate = predicate;
            return this;
        }

        internal Func<string, bool>? GetPredicate() => _predicate;
    }
}
