using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Audio
{
    public sealed class AudioWord
    {
        [JsonPropertyName("word")]
        public string? Word { get; set; }
        [JsonPropertyName("start")]
        public double Start { get; set; }
        [JsonPropertyName("end")]
        public double End { get; set; }
    }
}
