using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepFileSearchResult
    {
        [JsonPropertyName("file_id")]
        public string? FileId { get; set; }

        [JsonPropertyName("file_name")]
        public string? FileName { get; set; }

        [JsonPropertyName("score")]
        public float Score { get; set; }

        [JsonPropertyName("content")]
        public List<RunStepFileSearchContent>? Content { get; set; }
    }
}
