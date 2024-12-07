using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageResponse
    {
        [JsonIgnore]
        public ChatRole Role { get; set; }
        [JsonPropertyName("role")]
        public string StringableRole
        {
            get => Role.AsString();
            set => Role = (ChatRole)Enum.Parse(typeof(ChatRole), $"{value.ToUpper()[0]}{value.ToLower()[1..]}");
        }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("content")]
        public string? Content { get; set; }
        [JsonPropertyName("tool_calls")]
        public List<ChatMessageTool>? ToolCalls { get; set; }
        [JsonPropertyName("tool_call_id")]
        public string? ToolCallId { get; set; }
    }
}
