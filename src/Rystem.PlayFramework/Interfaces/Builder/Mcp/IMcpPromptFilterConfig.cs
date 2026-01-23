namespace Rystem.PlayFramework
{
    /// <summary>
    /// Configuration interface for MCP prompt filtering
    /// </summary>
    public interface IMcpPromptFilterConfig
    {
        /// <summary>
        /// Whether prompts are enabled
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Only prompts matching this regex pattern are available
        /// </summary>
        IMcpPromptFilterConfig WithPromptsMatching(string regexPattern);

        /// <summary>
        /// Only prompts starting with this prefix are available
        /// </summary>
        IMcpPromptFilterConfig WithPromptsStartingWith(string prefix);

        /// <summary>
        /// Only these specific prompt names are available
        /// </summary>
        IMcpPromptFilterConfig WithPrompts(params string[] names);

        /// <summary>
        /// All prompts EXCEPT these are available
        /// </summary>
        IMcpPromptFilterConfig ExcludingPrompts(params string[] names);

        /// <summary>
        /// Custom predicate for filtering
        /// </summary>
        IMcpPromptFilterConfig WithPromptsPredicate(Func<string, bool> predicate);
    }
}
