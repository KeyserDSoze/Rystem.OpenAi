using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Client event: conversation.item.delete.
    /// </summary>
    public class RealTimeClientConversationItemDeleteEvent : RealTimeClientEvent
    {
        [JsonPropertyName("item_id")]
        public string ItemId { get; set; }
    }
}
