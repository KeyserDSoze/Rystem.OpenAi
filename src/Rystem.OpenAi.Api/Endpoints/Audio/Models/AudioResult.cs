using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Image
{
    public sealed class AudioResult
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
