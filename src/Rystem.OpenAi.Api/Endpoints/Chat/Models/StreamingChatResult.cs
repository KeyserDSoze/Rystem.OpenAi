using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class StreamingChatResult
    {
        [JsonPropertyName("chunk")]
        public ChatResult Chunk { get; set; } = null!;
        [JsonPropertyName("composed")]
        public ChatResult Composed { get; set; } = null!;
    }
}
