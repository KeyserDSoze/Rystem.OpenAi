using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class ToolResourcesCodeInterpreterThreadResponse
    {
        [JsonPropertyName("file_ids")]
        public List<string>? Files { get; set; }
    }
}
