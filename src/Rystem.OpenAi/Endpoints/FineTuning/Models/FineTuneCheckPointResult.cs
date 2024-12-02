using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneCheckPointResult
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("created_at")]
        public int CreatedAt { get; set; }

        [JsonPropertyName("fine_tuned_model_checkpoint")]
        public string? FineTunedModelCheckpoint { get; set; }

        [JsonPropertyName("fine_tuning_job_id")]
        public string? FineTuningJobId { get; set; }

        [JsonPropertyName("metrics")]
        public FineTuneCheckPointMetrics? Metrics { get; set; }

        [JsonPropertyName("step_number")]
        public int? StepNumber { get; set; }
    }
}
