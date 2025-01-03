using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class LastErrorRun
    {
        [JsonPropertyName("code")]
        public string? CodeAsString { get; set; }
        [JsonIgnore]
        public LastErrorCode? Code => LastErrorCodeExtensions.ToLastErrorCode(CodeAsString);
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
