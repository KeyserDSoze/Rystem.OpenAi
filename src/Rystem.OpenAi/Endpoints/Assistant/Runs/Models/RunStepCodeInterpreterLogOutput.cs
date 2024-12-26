using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepCodeInterpreterLogOutput
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("logs")]
        public string? Logs { get; set; }
    }
}
