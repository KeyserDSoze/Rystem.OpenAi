using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantFileSearchTool
    {
        private const string FileType = "file_search";
        [JsonPropertyName("type")]
        [JsonAnyOfChooser(FileType)]
        public string Type { get; set; } = FileType;
        [JsonPropertyName("file_search")]
        public AssistantSettingsForFileSearchTool? FileSearch { get; set; }
    }
}
