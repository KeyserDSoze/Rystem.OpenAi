using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Embedding
{
    public sealed class EmbeddingRequest : IOpenAiRequest
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        [JsonPropertyName("input")]
        public object? Input { get; set; }
        [JsonPropertyName("user")]
        public string? User { get; set; }

        [JsonPropertyName("dimensions")]
        public int? Dimensions { get; set; }
    }
}
