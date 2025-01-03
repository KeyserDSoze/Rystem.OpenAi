using System;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class VectorStoreFile
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("usage_bytes")]
        public long UsageBytes { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("vector_store_id")]
        public string? VectorStoreId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("last_error")]
        public VectorStoreFileLastError? LastError { get; set; }

        [JsonPropertyName("chunking_strategy")]
        public AnyOf<VectorStoreFilChunkingStrategy, VectorStoreFilStaticChunkingStrategy>? ChunkingStrategy { get; set; }
    }
}
