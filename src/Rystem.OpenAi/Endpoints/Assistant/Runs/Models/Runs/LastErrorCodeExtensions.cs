namespace Rystem.OpenAi.Assistant
{
    internal static class LastErrorCodeExtensions
    {
        public static LastErrorCode? ToLastErrorCode(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            return input switch
            {
                "server_error" => LastErrorCode.ServerError,
                "rate_limit_exceeded" => LastErrorCode.RateLimitExceeded,
                "invalid_prompt" => LastErrorCode.InvalidPrompt,
                _ => null
            };
        }
    }
}
