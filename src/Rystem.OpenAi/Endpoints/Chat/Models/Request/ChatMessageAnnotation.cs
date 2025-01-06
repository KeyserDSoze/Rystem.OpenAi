using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageAnnotation
    {
        [JsonPropertyName("index")]
        public int? Index { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("text")]
        public string? Text { get; set; }
        [JsonPropertyName("start_index")]
        public int? StartIndex { get; set; }
        [JsonPropertyName("end_index")]
        public int? EndIndex { get; set; }
        [JsonPropertyName("file_citation")]
        public ChatMessageAnnotationFileCitation? FileCitation { get; set; }
    }
}
