using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepCodeInterpreterImageOutput
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("image")]
        public RunStepImage? Image { get; set; }
    }
}
