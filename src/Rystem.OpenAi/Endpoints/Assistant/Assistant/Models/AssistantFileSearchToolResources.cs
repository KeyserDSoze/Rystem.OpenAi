using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantFileSearchToolResources
    {
        [JsonPropertyName("vector_store_ids")]
        public List<string>? VectorStoresId { get; set; }
        [JsonPropertyName("vector_stores")]
        public AssistantVectorStoresFileSearchToolResources? VectorStores { get; set; }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
