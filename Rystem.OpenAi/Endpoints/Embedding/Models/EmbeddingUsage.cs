using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Embedding
{
    public sealed class EmbeddingUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

}
