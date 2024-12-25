using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class ThreadRequest : IOpenAiRequest
    {
        [JsonPropertyName("messages")]
        public List<ThreadMessage>? Messages { get; set; }
        [JsonPropertyName("tool_resources")]
        public AssistantToolResources? ToolResources { get; set; }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
        public string? Model { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
