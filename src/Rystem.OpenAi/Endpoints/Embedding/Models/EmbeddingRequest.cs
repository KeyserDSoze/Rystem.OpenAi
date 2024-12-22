using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Embedding
{
    public sealed class EmbeddingRequest : IOpenAiRequest
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        [JsonPropertyName("input")]
        public AnyOf<string, List<string>>? Input { get; set; }
        [JsonPropertyName("user")]
        public string? User { get; set; }
        [JsonPropertyName("dimensions")]
        public int? Dimensions { get; set; }
        [JsonPropertyName("encoding_format")]
        public string? EncodingFormat { get; set; }
    }
}
