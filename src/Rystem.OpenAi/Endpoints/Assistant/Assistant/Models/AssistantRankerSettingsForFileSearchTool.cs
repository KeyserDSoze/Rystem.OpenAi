using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantRankerSettingsForFileSearchTool
    {
        [JsonPropertyName("ranker")]
        public string? Ranker { get; set; }
        [JsonPropertyName("score_threshold")]
        public int ScoreThreshold { get; set; }
    }
}
