using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class VectorStoreFileCounts
    {
        [JsonPropertyName("in_progress")]
        public int InProgress { get; set; }

        [JsonPropertyName("completed")]
        public int Completed { get; set; }

        [JsonPropertyName("cancelled")]
        public int Cancelled { get; set; }

        [JsonPropertyName("failed")]
        public int Failed { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
