using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class StreamOptionsChatRequest
    {
        [JsonPropertyName("include_usage")]
        public bool IncludeUsage { get; set; }
    }
}
