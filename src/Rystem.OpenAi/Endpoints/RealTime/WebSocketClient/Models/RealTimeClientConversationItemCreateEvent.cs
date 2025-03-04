using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Client event: conversation.item.create.
    /// </summary>
    public class RealTimeClientConversationItemCreateEvent : RealTimeClientEvent
    {
        [JsonPropertyName("previous_item_id")]
        public string PreviousItemId { get; set; }

        [JsonPropertyName("item")]
        public RealTimeClientConversationItem Item { get; set; }
    }
}
