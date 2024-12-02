using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ForcedFunctionTool
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("function")]
        public ForcedFunctionTool? Function { get; set; }
    }
}
