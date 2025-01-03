using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class VectorStoreFileRequest
    {
        [JsonPropertyName("file_id")]
        public string? Id { get; set; }
        [JsonPropertyName("file_ids")]
        public List<string>? Ids { get; set; }
        [JsonPropertyName("chunking_strategy")]
        public AssistantChunkingStrategyVectorStoresFileSearchToolResources? ChunkingStrategy { get; set; }
    }
}
