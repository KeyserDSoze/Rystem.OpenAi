using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public interface IAssistantToolResources
    {
        AssistantToolResources? ToolResources { get; set; }
    }
    public sealed class ThreadRequest : IOpenAiRequest, IAssistantToolResources
    {
        [JsonPropertyName("messages")]
        public List<ThreadMessages>? Messages { get; set; }
        [JsonPropertyName("tool_resources")]
        public AssistantToolResources? ToolResources { get; set; }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
        public string? Model { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
