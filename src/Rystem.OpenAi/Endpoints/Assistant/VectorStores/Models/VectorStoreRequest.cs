using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class VectorStoreRequest : IOpenAiRequestWithMetadata
    {
        [JsonPropertyName("file_ids")]
        public List<string>? FileIds { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("expires_after")]
        public VectoStoreExpirationPolicy? ExpiresAfter { get; set; }
        [JsonPropertyName("chunking_strategy")]
        public AssistantChunkingStrategyVectorStoresFileSearchToolResources? ChunkingStrategy { get; set; }
        [JsonIgnore]
        public string? Model { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
