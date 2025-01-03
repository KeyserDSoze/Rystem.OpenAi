using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepFileSearch
    {
        [JsonPropertyName("ranking_options")]
        public RunStepRankingOptions? RankingOptions { get; set; }

        [JsonPropertyName("results")]
        public List<RunStepFileSearchResult>? Results { get; set; }
    }
}
