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
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public List<RealTimeClientConversationContent> Content { get; set; }
    }
}
