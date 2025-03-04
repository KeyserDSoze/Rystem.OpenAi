using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Client event: session.update.
    /// </summary>
    public class RealTimeClientSessionUpdateEvent : RealTimeClientEvent
    {
        [JsonPropertyName("session")]
        public RealTimeClientSessionUpdateData Session { get; set; }
    }
}
