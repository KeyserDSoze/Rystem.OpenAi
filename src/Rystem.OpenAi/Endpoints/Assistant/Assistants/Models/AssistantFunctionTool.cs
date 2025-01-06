using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantFunctionTool
    {
        private const string FunctionType = "function";
        [JsonPropertyName("type")]
        [AnyOfJsonSelector(FunctionType)]
        public string Type { get; set; } = FunctionType;
        [JsonPropertyName("function")]
        public FunctionTool? Function { get; set; }
    }
}
