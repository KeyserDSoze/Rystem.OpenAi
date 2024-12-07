using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class PredictionChatRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("content")]
        public object? Content { get; set; }
    }
}
