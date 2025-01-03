using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class VectorStoreFilChunkingStrategy
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
