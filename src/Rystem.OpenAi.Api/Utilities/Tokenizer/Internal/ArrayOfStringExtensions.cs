namespace Rystem.OpenAi
{
    internal static class ArrayOfStringExtensions
    {
        public static object ToCorrectPrompt(this string[]? prompts)
        {
            if (prompts == null)
                return string.Empty;
            if (prompts.Length > 1)
                return prompts;
            if (prompts.Length == 1)
                return prompts[0];
            return string.Empty;
        }
    }
}
