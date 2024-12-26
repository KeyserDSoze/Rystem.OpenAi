using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class VectorStoreFilStaticChunkingDetails
    {
        [JsonPropertyName("max_chunk_size_tokens")]
        public int MaxChunkSizeTokens { get; set; }

        [JsonPropertyName("chunk_overlap_tokens")]
        public int ChunkOverlapTokens { get; set; }
    }
}
