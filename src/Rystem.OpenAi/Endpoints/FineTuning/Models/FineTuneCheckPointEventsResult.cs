using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneCheckPointEventsResult
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("data")]
        public List<FineTuneCheckPointResult>? Data { get; set; }
        [JsonPropertyName("first_id")]
        public string? FirstId { get; set; }
        [JsonPropertyName("last_id")]
        public string? LastId { get; set; }
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }
}
