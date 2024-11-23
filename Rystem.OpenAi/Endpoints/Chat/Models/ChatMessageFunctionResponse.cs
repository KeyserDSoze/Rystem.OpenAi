using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageFunctionResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("arguments")]
        public string? Arguments { get; set; }
    }
}
