using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatChoiceToolRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("function")]
        public ChatChoiceFunctionRequest? Function { get; set; }
    }
}
