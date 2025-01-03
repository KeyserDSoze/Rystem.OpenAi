using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class ToolOutputRequest
    {
        [JsonPropertyName("tool_outputs")]
        public List<ToolOutput>? ToolOutputs { get; set; }

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
    }
}
