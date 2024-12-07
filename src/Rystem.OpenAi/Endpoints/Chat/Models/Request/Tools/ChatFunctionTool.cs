using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatFunctionTool
    {
        private const string FunctionType = "function";
        [JsonPropertyName("type")]
        public string Type { get; } = FunctionType;
        [JsonPropertyName("function")]
        public required FunctionTool Function { get; set; }
    }
}
