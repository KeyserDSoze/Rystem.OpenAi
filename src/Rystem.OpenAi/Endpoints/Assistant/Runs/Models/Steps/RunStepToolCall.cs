using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepToolCall
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("code_interpreter")]
        public RunStepCodeInterpreter? CodeInterpreter { get; set; }

        [JsonPropertyName("file_search")]
        public RunStepFileSearch? FileSearch { get; set; }

        [JsonPropertyName("function")]
        public RunStepFunctionTool? Function { get; set; }
    }
}
