using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneRequest : IOpenAiRequest
    {
        [JsonPropertyName("model")]
        public string? ModelId { get; set; }
        [JsonPropertyName("training_file")]
        public string? TrainingFile { get; set; }
        [JsonPropertyName("validation_file")]
        public string? ValidationFile { get; set; }
        [JsonPropertyName("hyperparameters")]
        public FineTuneRequestHyperparameters? Hyperparameters { get; set; }
        [JsonPropertyName("suffix")]
        public string? Suffix { get; set; }
    }
}
