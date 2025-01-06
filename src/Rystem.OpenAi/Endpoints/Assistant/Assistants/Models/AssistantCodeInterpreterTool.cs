using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantCodeInterpreterTool
    {
        private const string FileType = "code_interpreter";
        [JsonPropertyName("type")]
        [AnyOfJsonSelector(FileType)]
        public string Type { get; set; } = FileType;
    }
}
