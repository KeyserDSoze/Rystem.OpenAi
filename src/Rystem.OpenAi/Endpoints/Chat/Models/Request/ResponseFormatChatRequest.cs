using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ResponseFormatChatRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("content")]
        public FunctionTool? Content { get; set; }
    }
}
