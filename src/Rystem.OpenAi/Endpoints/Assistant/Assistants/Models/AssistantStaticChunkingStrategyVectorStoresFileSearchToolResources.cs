using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantStaticChunkingStrategyVectorStoresFileSearchToolResources
    {
        [JsonPropertyName("max_chunk_size_tokens")]
        public int MaxChunkSizeTokens { get; set; } = 800;
        [JsonPropertyName("chunk_overlap_tokens")]
        public int ChunkOverlapTokens { get; set; } = 400;
    }
}
