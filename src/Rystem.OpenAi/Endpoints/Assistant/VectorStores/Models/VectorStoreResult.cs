using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class VectorStoreResult : ApiBaseResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("usage_bytes")]
        public int? UsageBytes { get; set; }
        [JsonPropertyName("last_active_at")]
        public int? LastActiveAt { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        //todo: adding a status as enum like the others status property
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        [JsonPropertyName("file_counts")]
        public VectorStoreFileCounts? FileCounts { get; set; }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
        [JsonPropertyName("last_used_at")]
        public int? LastUsedAt { get; set; }
    }
}
