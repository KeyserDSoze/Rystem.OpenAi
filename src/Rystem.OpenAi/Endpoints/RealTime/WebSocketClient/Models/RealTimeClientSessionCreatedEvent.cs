using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    // ─── EXAMPLE SERVER EVENT CLASS ───────────────────────────────────

    /// <summary>
    /// Example: Event emitted when a session is created.
    /// </summary>
    public class RealTimeClientSessionCreatedEvent : RealTimeClientEvent
    {
        [JsonPropertyName("session")]
        public RealTimeSessionResponse? Session { get; set; }
    }
}
