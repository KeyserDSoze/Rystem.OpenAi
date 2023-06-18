using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatFunctionParameters : ChatFunctionProperty
    {
        [JsonPropertyName("properties")]
        public Dictionary<string, ChatFunctionProperty> Properties { get; set; } = null!;
        [JsonPropertyName("required")]
        public List<string>? Required { get; set; }
    }
}
