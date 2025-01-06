using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageTextContent
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }
        [JsonPropertyName("annotations")]
        public List<ChatMessageAnnotation>? Annotations { get; set; }
    }
}
