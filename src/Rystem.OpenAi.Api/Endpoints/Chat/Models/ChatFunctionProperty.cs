using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatFunctionNonPrimitiveProperty : ChatFunctionProperty
    {
        [JsonPropertyName("properties")]
        public Dictionary<string, ChatFunctionProperty> Properties { get; set; } = null!;
    }
    [JsonDerivedType(typeof(ChatFunctionEnumProperty))]
    [JsonDerivedType(typeof(ChatFunctionNonPrimitiveProperty))]
    public class ChatFunctionProperty
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
    public sealed class ChatFunctionEnumProperty : ChatFunctionProperty
    {
        [JsonPropertyName("enum")]
        public List<string>? Enums { get; set; }
    }
}
