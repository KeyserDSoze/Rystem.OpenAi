using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantFunctionTool
    {
        private const string FunctionType = "function";
        [JsonPropertyName("type")]
        public string Type { get; } = FunctionType;
        [JsonPropertyName("function")]
        public required FunctionTool Function { get; set; }
    }
}
