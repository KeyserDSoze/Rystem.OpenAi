using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Ephemeral client secret details.
    /// </summary>
    public class ClientSecret
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("expires_at")]
        public long? ExpiresAt { get; set; }
    }
}
