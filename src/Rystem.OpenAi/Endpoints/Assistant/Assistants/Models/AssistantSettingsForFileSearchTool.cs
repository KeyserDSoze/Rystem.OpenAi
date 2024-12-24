using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantSettingsForFileSearchTool
    {
        [JsonPropertyName("max_num_results")]
        public int MaxNumberOfResults { get; set; }
        [JsonPropertyName("ranking_options")]
        public AssistantRankerSettingsForFileSearchTool? RankingOptions { get; set; }
    }
}
