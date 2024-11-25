using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class WeightsAndBiasesFineTuneIntegration
    {
        [JsonPropertyName("project")]
        public string? Project { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("entity")]
        public string? Entity { get; set; }
        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }
    }
}
