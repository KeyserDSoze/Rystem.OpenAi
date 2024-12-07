using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ForceFunctionToolName
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}
