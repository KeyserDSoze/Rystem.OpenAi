using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Configuration for input audio transcription.
    /// </summary>
    public class RealTimeInputAudioTranscriptionRequest
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("prompt")]
        public string? Prompt { get; set; }
    }
}
