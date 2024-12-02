using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class TextPredictionChatRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
