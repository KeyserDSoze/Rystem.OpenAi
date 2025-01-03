using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepFunctionTool
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("arguments")]
        public string? Arguments { get; set; }
        [JsonPropertyName("output")]
        public string? Output { get; set; }
    }
}
