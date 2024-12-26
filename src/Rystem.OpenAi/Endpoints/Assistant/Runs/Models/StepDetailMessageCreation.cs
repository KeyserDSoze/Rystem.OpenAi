using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class StepDetailMessageCreation
    {
        [JsonPropertyName("message_id")]
        public string? MessageId { get; set; }
    }
}
