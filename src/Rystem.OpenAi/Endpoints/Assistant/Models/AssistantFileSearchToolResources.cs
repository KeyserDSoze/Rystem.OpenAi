using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantFileSearchToolResources
    {
        [JsonPropertyName("vector_store_ids")]
        public List<string>? vectorStores { get; set; }
    }
}
