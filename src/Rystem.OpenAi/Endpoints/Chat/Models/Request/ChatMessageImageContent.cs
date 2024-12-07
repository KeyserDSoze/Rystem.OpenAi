using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageImageContent
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }
        [JsonPropertyName("detail")]
        public string? Detail { get; set; }
    }
}
