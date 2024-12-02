using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneIntegration
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("wandb")]
        public object? WeightsAndBiases { get; set; }
    }
}
