using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneCheckPointMetrics
    {
        [JsonPropertyName("step")]
        public int Step { get; set; }

        [JsonPropertyName("train_loss")]
        public double TrainLoss { get; set; }

        [JsonPropertyName("train_mean_token_accuracy")]
        public double TrainMeanTokenAccuracy { get; set; }

        [JsonPropertyName("valid_loss")]
        public double ValidLoss { get; set; }

        [JsonPropertyName("valid_mean_token_accuracy")]
        public double ValidMeanTokenAccuracy { get; set; }

        [JsonPropertyName("full_valid_loss")]
        public double FullValidLoss { get; set; }

        [JsonPropertyName("full_valid_mean_token_accuracy")]
        public double FullValidMeanTokenAccuracy { get; set; }
    }
}
