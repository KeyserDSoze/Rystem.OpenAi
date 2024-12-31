using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    public class ThreadMessage : IOpenAiRequest
    {
        [JsonPropertyName("role")]
        public string? Role { get; set; }
        [JsonPropertyName("content")]
        public AnyOf<string, List<ChatMessageContent>>? Content { get; set; }
        [JsonPropertyName("attachments")]
        public List<ThreadAttachment>? Attachments { get; set; }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
        [JsonIgnore]
        public string? Model { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
