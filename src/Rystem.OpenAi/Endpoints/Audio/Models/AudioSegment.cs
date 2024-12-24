using System;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Endpoints.Audio.Models;

namespace Rystem.OpenAi.Audio
{
    public sealed class AudioSegment
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("seek"), JsonConverter(typeof(NumberToTimeSpanConverter))]
        public TimeSpan Seek { get; set; }

        [JsonPropertyName("start"), JsonConverter(typeof(NumberToTimeSpanConverter))]
        public TimeSpan Start { get; set; }

        [JsonPropertyName("end"), JsonConverter(typeof(NumberToTimeSpanConverter))]
        public TimeSpan End { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("tokens")]
        public int[]? Tokens { get; set; }

        [JsonPropertyName("temperature")]
        public float Temperature { get; set; }

        [JsonPropertyName("avg_logprob")]
        public float AvgLogprob { get; set; }

        [JsonPropertyName("compression_ratio")]
        public float CompressionRatio { get; set; }

        [JsonPropertyName("no_speech_prob")]
        public float NoSpeechProb { get; set; }
    }
}
