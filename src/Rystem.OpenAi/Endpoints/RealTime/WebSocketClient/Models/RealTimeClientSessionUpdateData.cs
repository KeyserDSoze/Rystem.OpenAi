using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    // ─── CLIENT EVENT PAYLOAD CLASSES ───────────────────────────────

    /// <summary>
    /// Payload for the session.update client event.
    /// </summary>
    public class RealTimeClientSessionUpdateData
    {
        [JsonPropertyName("modalities")]
        public string[]? Modalities { get; set; }

        [JsonPropertyName("instructions")]
        public string? Instructions { get; set; }

        [JsonPropertyName("voice")]
        public string? Voice { get; set; }

        [JsonPropertyName("input_audio_format")]
        public string? InputAudioFormat { get; set; }

        [JsonPropertyName("output_audio_format")]
        public string? OutputAudioFormat { get; set; }
    }
}
