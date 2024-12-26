using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class IncompleteReasonRun
    {
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
}
