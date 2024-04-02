using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class Hyperparams
    {
        [JsonPropertyName("batch_size")]
        public int BatchSize { get; set; }
        [JsonPropertyName("learning_rate_multiplier")]
        public float LearningRateMultiplier { get; set; }
        [JsonPropertyName("n_epochs")]
        public int NEpochs { get; set; }
        [JsonPropertyName("prompt_loss_weight")]
        public float PromptLossWeight { get; set; }
    }
}
