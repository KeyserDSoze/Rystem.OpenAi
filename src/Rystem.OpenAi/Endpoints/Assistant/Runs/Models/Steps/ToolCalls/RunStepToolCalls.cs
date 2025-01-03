using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepToolCalls
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("tool_calls")]
        public List<RunStepToolCall>? Tools { get; set; }
    }
}
