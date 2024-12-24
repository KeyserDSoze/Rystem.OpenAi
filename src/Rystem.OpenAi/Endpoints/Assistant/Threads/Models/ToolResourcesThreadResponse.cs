using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class ToolResourcesThreadResponse
    {
        [JsonPropertyName("code_interpreter")]
        public ToolResourcesCodeInterpreterThreadResponse? CodeInterpreter { get; set; }
        [JsonPropertyName("file_search")]
        public ToolResourcesFileSearchThreadResponse? FileSearch { get; set; }

    }
}
