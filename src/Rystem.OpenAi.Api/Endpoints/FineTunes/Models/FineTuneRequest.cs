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
        [JsonPropertyName("n_epochs")]
        public int NEpochs { get; set; }
        [JsonPropertyName("batch_size")]
        public int BatchSize { get; set; }
        [JsonPropertyName("learning_rate_multiplier")]
        public double LearningRateMultiplier { get; set; }
        [JsonPropertyName("prompt_loss_weight")]
        public double PromptLossWeight { get; set; }
        [JsonPropertyName("compute_classification_metrics")]
        public bool ComputeClassificationMetrics { get; set; }
        [JsonPropertyName("classification_n_classes")]
        public int? ClassificationNClasses { get; set; }
        [JsonPropertyName("classification_positive_class")]
        public string? ClassificationPositiveClass { get; set; }
        [JsonPropertyName("classification_betas")]
        public string[]? ClassificationBetas { get; set; }
        [JsonPropertyName("suffix")]
        public string? Suffix { get; set; }
    }
}
