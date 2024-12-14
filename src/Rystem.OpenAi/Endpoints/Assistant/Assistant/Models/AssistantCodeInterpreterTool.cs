using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantCodeInterpreterTool
    {
        private const string FileType = "code_interpreter";
        [JsonPropertyName("type")]
        public string Type { get; } = FileType;
    }
}
