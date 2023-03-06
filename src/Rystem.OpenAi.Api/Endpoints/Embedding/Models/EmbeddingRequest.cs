using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Embedding
{
    public sealed class EmbeddingRequest : IOpenAiRequest
    {
        [JsonPropertyName("model")]
        public string? ModelId { get; set; }
        [JsonPropertyName("input")]
        public object? Input { get; set; }
        [JsonPropertyName("user")]
        public string? User { get; set; }
    }
}
