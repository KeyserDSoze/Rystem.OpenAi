using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Image
{
    public sealed class VerboseAudioResult
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("langauge")]
        public string? Language { get; set; }

        [JsonPropertyName("duration")]
        public double Duration { get; set; }

        [JsonPropertyName("segments")]
        public AudioSegment[]? Segments { get; set; }

        public sealed class AudioSegment
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("seek")]
            public int Seek { get; set; }

            [JsonPropertyName("start")]
            public double Start { get; set; }

            [JsonPropertyName("end")]
            public double End { get; set; }

            [JsonPropertyName("text")]
            public string? Text { get; set; }

            [JsonPropertyName("tokens")]
            public int[]? Tokens { get; set; }

            [JsonPropertyName("temperature")]
            public double Temperature { get; set; }

            [JsonPropertyName("avg_logprob")]
            public double AvgLogprob { get; set; }

            [JsonPropertyName("compression_ratio")]
            public double CompressionRatio { get; set; }

            [JsonPropertyName("no_speech_prob")]
            public double NoSpeechProb { get; set; }
        }
    }
}
