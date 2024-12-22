using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantCodeInterpreterToolResources
    {
        [JsonPropertyName("file_ids")]
        public List<string>? Files { get; set; }
    }
}
