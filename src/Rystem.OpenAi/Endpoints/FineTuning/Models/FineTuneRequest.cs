using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneRequest : IOpenAiRequest
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        [JsonPropertyName("training_file")]
        public string? TrainingFile { get; set; }
        [JsonPropertyName("validation_file")]
        public string? ValidationFile { get; set; }
        [JsonPropertyName("hyperparameters")]
        public FineTuneHyperParameters? Hyperparameters { get; set; }
        [JsonPropertyName("suffix")]
        public string? Suffix { get; set; }
        [JsonPropertyName("integrations")]
        public List<FineTuneIntegration>? Integrations { get; set; }
        [JsonPropertyName("seed")]
        public int? Seed { get; set; }
    }
}
