using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageAudioFile
    {
        [JsonPropertyName("data")]
        public string? Data { get; set; }
        [JsonPropertyName("format")]
        public string? Format { get; set; }
    }
}
