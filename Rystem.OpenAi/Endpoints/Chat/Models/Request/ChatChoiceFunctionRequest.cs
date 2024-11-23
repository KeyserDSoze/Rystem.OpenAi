using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatChoiceFunctionRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
