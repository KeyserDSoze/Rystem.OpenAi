using System;
using System.Text;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessage
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
        [JsonPropertyName("function_call")]
        public ChatMessageFunction? Function { get; set; }
        private StringBuilder? _content;
        internal void AddContent(string? content)
        {
            if (content == null)
                return;
            _content ??= new StringBuilder();
            _content.Append(content);
        }
        internal void BuildContent()
            => Content = _content?.ToString() ?? string.Empty;
    }
}
