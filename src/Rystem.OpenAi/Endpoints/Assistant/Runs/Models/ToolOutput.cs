using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class ToolOutput
    {
        [JsonPropertyName("tool_call_id")]
        public string? ToolCallId { get; set; }

        [JsonPropertyName("output")]
        public string? Output { get; set; }
    }
}
