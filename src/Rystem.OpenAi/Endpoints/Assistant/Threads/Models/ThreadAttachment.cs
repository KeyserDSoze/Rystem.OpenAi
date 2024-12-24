using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class ThreadAttachment
    {
        [JsonPropertyName("file_id")]
        public string? FileId { get; set; }
        [JsonPropertyName("tools")]
        public List<ThreadAttachmentTool>? Tools { get; set; }
    }
}
