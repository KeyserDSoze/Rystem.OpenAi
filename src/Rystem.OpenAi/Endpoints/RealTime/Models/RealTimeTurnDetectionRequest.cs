using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Configuration for turn detection.
    /// </summary>
    public class RealTimeTurnDetectionRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; } = "server_vad";

        [JsonPropertyName("threshold")]
        public double? Threshold { get; set; }

        [JsonPropertyName("prefix_padding_ms")]
        public int? PrefixPaddingMs { get; set; }

        [JsonPropertyName("silence_duration_ms")]
        public int? SilenceDurationMs { get; set; }

        [JsonPropertyName("create_response")]
        public bool CreateResponse { get; set; } = true;
    }
}
