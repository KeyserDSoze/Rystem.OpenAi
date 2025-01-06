using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageTextContent
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }
        [JsonPropertyName("annotations")]
        public List<string>? Annotations { get; set; }
    }
}
