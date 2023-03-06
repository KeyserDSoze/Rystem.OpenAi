using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    /// <summary>
    /// API usage as reported by the OpenAI API for this request
    /// </summary>
    public class ChatUsage : Usage
    {
        /// <summary>
        /// How many tokens are in the completion(s)
        /// </summary>
        [JsonPropertyName("completion_tokens")]
        public short? CompletionTokens { get; set; }
    }

}
