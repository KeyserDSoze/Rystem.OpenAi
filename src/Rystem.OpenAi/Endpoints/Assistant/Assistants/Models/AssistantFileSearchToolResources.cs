using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantFileSearchToolResources
    {
        [JsonPropertyName("vector_store_ids")]
        public List<string>? VectorStoresId { get; set; }
        [JsonPropertyName("vector_stores")]
        public List<AssistantVectorStoresFileSearchToolResources>? VectorStores { get; set; }
    }
}
