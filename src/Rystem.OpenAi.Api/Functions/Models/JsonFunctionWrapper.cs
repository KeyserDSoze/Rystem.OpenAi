using Rystem.OpenAi.Chat;

namespace System.Text.Json.Serialization
{
    public sealed class JsonFunctionWrapper : ITool
    {
        [JsonPropertyName("type")]
        public string Type => ChatConstants.ToolType.Function;
        [JsonPropertyName("function")]
        public JsonFunction? Function { get; set; }
    }
}
