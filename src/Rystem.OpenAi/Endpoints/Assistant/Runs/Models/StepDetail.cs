using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class StepDetail
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("message_creation")]
        public StepDetailMessageCreation? MessageCreation { get; set; }
    }
}
