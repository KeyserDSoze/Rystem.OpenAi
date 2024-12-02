using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Audio
{
    public sealed class AudioSpeechRequest : IOpenAiRequest
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        [JsonPropertyName("input")]
        public string? Input { get; set; }
        [JsonPropertyName("voice")]
        public string? Voice { get; set; }
        [JsonPropertyName("response_format")]
        public string? ResponseFormat { get; set; }
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
    }
}
