using System;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Audio
{
    public sealed class VerboseSegmentAudioResult
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("duration"), JsonConverter(typeof(NumberToTimeSpanConverter))]
        public TimeSpan Duration { get; set; }

        [JsonPropertyName("segments")]
        public AudioSegment[]? Segments { get; set; }
    }
}
