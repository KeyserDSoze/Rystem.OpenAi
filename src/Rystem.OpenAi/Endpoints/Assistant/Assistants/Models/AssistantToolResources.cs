using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantToolResources
    {
        [JsonPropertyName("code_interpreter")]
        public AssistantCodeInterpreterToolResources? CodeInterpreter { get; set; }
        [JsonPropertyName("file_search")]
        public AssistantFileSearchToolResources? FileSearch { get; set; }
    }
}
