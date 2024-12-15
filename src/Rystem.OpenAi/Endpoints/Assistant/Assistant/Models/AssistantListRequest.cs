using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantListRequest
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("data")]
        public List<AssistantRequest>? Data { get; set; }
    }
}
