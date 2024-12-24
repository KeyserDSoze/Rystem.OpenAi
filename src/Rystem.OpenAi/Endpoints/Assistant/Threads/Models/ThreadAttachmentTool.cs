using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class ThreadAttachmentTool
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        public static ThreadAttachmentTool CodeInterpreter { get; } = new ThreadAttachmentTool
        {
            Type = "code_interpreter"
        };
        public static ThreadAttachmentTool FileSearch { get; } = new ThreadAttachmentTool
        {
            Type = "file_search"
        };
    }
}
