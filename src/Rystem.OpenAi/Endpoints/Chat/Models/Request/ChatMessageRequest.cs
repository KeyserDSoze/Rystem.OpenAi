using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageRequest
    {
        [JsonIgnore]
        public ChatRole Role { get; set; }
        [JsonPropertyName("role")]
        public string StringableRole
        {
            get => Role.AsString();
            set => Role = Enum.Parse<ChatRole>($"{value.ToUpper()[0]}{value.ToLower()[1..]}");
        }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("content")]
        public AnyOf<string, List<ChatMessageContent>>? Content { get; set; }
        [JsonPropertyName("tool_call_id")]
        public string? ToolCallId { get; set; }
    }
}
