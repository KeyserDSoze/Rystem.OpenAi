using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageAnnotationFileCitation
    {
        [JsonPropertyName("file_id")]
        public string? FileId { get; set; }

        [JsonPropertyName("quote")]
        public string? Quote { get; set; }
    }
}
