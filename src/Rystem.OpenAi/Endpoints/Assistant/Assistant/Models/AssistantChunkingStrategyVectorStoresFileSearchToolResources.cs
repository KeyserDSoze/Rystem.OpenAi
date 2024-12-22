using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantChunkingStrategyVectorStoresFileSearchToolResources
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("static")]
        public AssistantStaticChunkingStrategyVectorStoresFileSearchToolResources? Static { get; set; }
    }
}
