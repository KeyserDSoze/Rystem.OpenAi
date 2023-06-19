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

        private StringBuilder? _function;
        internal void AddFunction(string name)
        {
            if (name != null && Function == null)
            {
                Function = new ChatMessageFunction
                {
                    Name = name,
                    Arguments = string.Empty
                };
            }
        }
        internal void AddArgumentFunction(string? content)
        {
            if (content == null)
                return;
            _function ??= new StringBuilder();
            _function.Append(content);
        }
        internal void BuildFunction()
        {
            if (Function != null)
                Function.Arguments = _function?.ToString() ?? string.Empty;
        }
    }
}
