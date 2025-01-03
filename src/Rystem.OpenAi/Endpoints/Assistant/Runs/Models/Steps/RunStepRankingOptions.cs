using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepRankingOptions
    {
        [JsonPropertyName("ranker")]
        public string? Ranker { get; set; }

        [JsonPropertyName("score_threshold")]
        public float ScoreThreshold { get; set; }
    }
}
