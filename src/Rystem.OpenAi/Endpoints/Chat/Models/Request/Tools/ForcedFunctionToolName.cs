using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ForcedFunctionToolName
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
