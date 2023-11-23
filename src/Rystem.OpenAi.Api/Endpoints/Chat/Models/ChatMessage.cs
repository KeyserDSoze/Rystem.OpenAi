using System;
using System.Text;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("image_url")]
        public ChatMessageImageContent Image { get; set; }
    }
    public sealed class ChatMessageImageContent
    {
        [JsonPropertyName("url")]
        public string Type { get; set; }
        /// <summary>
        /// By controlling the detail parameter, which has three options, low, high, or auto, you have control over how the model processes the image and generates its textual understanding. By default, the model will use the auto setting which will look at the image input size and decide if it should use the low or high setting.
        /// low will disable the “high res” model. The model will receive a low-res 512px x 512px version of the image, and represent the image with a budget of 65 tokens. This allows the API to return faster responses and consume fewer input tokens for use cases that do not require high detail.
        /// high will enable “high res” mode, which first allows the model to see the low res image and then creates detailed crops of input images as 512px squares based on the input image size. Each of the detailed crops uses twice the token budget (65 tokens) for a total of 129 tokens.
        /// </summary>
        [JsonPropertyName("detail")]
        public string Detail { get; set; }
    }
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
        [JsonPropertyName("content")]
        public string? Content { get; set; }
        [JsonPropertyName("tool_calls")]
        public ChatMessageTool? ToolCall { get; set; }
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
        private const string FunctionLabel = "function";
        private StringBuilder? _function;
        internal void AddFunction(string name)
        {
            if (name != null && ToolCall == null)
            {
                ToolCall = new ChatMessageTool
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = FunctionLabel,
                    Function = new ChatMessageFunctionResponse
                    {
                        Name = name,
                        Arguments = string.Empty
                    }
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
            if (ToolCall?.Function != null)
                ToolCall.Function.Arguments = _function?.ToString() ?? string.Empty;
        }
    }
}
