using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneResults
    {
        [JsonPropertyName("data")]
        public List<FineTuneResult>? Data { get; set; }
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }
}
