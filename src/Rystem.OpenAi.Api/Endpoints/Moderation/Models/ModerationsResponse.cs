using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Moderation
{
    public sealed class ModerationsResponse : ApiBaseResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("results")]
        public List<ModerationResult>? Results { get; set; }
    }
}
