using System;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageContent
    {
        [JsonPropertyName("index")]
        public int? Index { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("text")]
        public AnyOf<string, ChatMessageTextContent>? Text { get; set; }
        [JsonPropertyName("image_url")]
        public ChatMessageImageContent? Image { get; set; }
        [JsonPropertyName("image_file")]
        public ChatMessageImageFile? FileImage { get; set; }
        [JsonPropertyName("input_audio")]
        public ChatMessageAudioFile? AudioInput { get; set; }
        [JsonPropertyName("refusal")]
        public string? Refusal { get; set; }
    }
}
