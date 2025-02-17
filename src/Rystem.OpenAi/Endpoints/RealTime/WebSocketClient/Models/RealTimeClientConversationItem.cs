using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Represents a conversation item.
    /// </summary>
    public class RealTimeClientConversationItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("role")]
        public string? Role { get; set; }
        [JsonPropertyName("content")]
        public List<RealTimeClientConversationContent>? Content { get; set; }
        [JsonPropertyName("call_id")]
        public string? CallId { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("arguments")]
        public string? Arguments { get; set; }
        [JsonPropertyName("output")]
        public string? Output { get; set; }
    }
}
