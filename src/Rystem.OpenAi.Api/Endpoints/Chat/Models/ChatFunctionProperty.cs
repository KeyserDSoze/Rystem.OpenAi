using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatFunctionProperty
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("enum")]
        public List<string>? Enums { get; set; }
    }
}
