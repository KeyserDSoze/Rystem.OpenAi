using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    /// <summary>
    /// Detailed breakdown of prompt tokens usage
    /// </summary>
    public sealed class PromptTokensDetails
    {
        /// <summary>
        /// Number of tokens from the prompt that were cached and reused from previous requests
        /// This reduces cost and improves performance
        /// </summary>
        [JsonPropertyName("cached_tokens")]
        public int? CachedTokens { get; set; }

        /// <summary>
        /// Number of audio tokens in the prompt (for audio inputs)
        /// </summary>
        [JsonPropertyName("audio_tokens")]
        public int? AudioTokens { get; set; }
    }
}
