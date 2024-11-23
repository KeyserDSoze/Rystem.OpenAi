using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class AudioChatRequest
    {
        [JsonPropertyName("voice")]
        public string? Voice { get; set; }
        [JsonPropertyName("format")]
        public string? Format { get; set; }
    }
}
