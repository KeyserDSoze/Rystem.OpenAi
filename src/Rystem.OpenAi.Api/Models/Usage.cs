using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    /// <summary>
    /// Usage statistics of how many tokens have been used for this request.
    /// </summary>
    public class Usage
    {
        /// <summary>
        /// How many tokens did the prompt consist of
        /// </summary>
        [JsonPropertyName("prompt_tokens")]
        public int? PromptTokens { get; set; }
        /// <summary>
        /// How many tokens did the request consume total
        /// </summary>
        [JsonPropertyName("total_tokens")]
        public int? TotalTokens { get; set; }
    }
}
