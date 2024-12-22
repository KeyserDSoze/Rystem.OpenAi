using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantVectorStoresFileSearchToolResources
    {
        [JsonPropertyName("file_ids")]
        public List<string>? Files { get; set; }
        [JsonPropertyName("chunking_strategy")]
        public AssistantChunkingStrategyVectorStoresFileSearchToolResources? ChunkingStrategy { get; set; }
    }
}
