using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class StreamingChatResult
    {
        [JsonPropertyName("current")]
        public ChatResult Current { get; set; } = null!;
        [JsonPropertyName("composed")]
        public ChatResult Composed { get; set; } = null!;
    }
}
