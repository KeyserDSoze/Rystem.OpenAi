using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class VectoStoreExpirationPolicy
    {
        [JsonPropertyName("anchor")]
        public string? Anchor { get; set; }

        [JsonPropertyName("days")]
        public int Days { get; set; }
    }
}
