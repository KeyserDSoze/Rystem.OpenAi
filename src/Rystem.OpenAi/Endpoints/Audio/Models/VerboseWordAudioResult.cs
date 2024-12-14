using System;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Audio
{
    public sealed class VerboseWordAudioResult
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("duration"), JsonConverter(typeof(NumberToTimeSpanConverter))]
        public TimeSpan Duration { get; set; }

        [JsonPropertyName("words")]
        public AudioWord[]? Words { get; set; }
    }
}
