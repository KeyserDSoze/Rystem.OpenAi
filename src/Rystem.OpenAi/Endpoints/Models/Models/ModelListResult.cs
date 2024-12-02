using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public sealed class ModelListResult
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("data")]
        public List<ModelResult>? Models { get; set; }
    }
}
