using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Client event: conversation.item.truncate.
    /// </summary>
    public class RealTimeClientConversationItemTruncateEvent : RealTimeClientEvent
    {
        [JsonPropertyName("item_id")]
        public string ItemId { get; set; }

        [JsonPropertyName("content_index")]
        public int ContentIndex { get; set; }

        [JsonPropertyName("audio_end_ms")]
        public int AudioEndMs { get; set; }
    }
}
