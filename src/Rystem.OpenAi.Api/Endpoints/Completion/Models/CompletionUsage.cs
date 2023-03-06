using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Completion
{
    /// <summary>
    /// API usage as reported by the OpenAI API for this request
    /// </summary>
    public class CompletionUsage : Usage
    {
        /// <summary>
        /// How many tokens are in the completion(s)
        /// </summary>
        [JsonPropertyName("completion_tokens")]
        public short? CompletionTokens { get; set; }
    }
}
