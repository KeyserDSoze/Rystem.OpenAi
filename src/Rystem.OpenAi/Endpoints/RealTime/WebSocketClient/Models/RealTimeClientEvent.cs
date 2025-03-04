using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    // ─── BASE EVENT CLASS ─────────────────────────────────────────────

    /// <summary>
    /// Base class for all realtime events.
    /// </summary>
    public class RealTimeClientEvent
    {
        [JsonPropertyName("event_id")]
        public string? EventId { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
