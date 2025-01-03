using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageImageFile
    {
        [JsonPropertyName("file_id")]
        public string? FileId { get; set; }
        [JsonPropertyName("detail")]
        public string? Detail { get; set; }
    }
}
