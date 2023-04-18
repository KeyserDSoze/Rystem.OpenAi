using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    /// <summary>
    /// Represents a completion choice returned by the Chat API.  
    /// </summary>
    public class ChatChoice
    {
        /// <summary>
        /// Messages.
        /// </summary>
        [JsonPropertyName("message")]
        public ChatMessage? Message { get; set; }
        /// <summary>
        /// If multiple completion choices we returned, this is the index withing the various choices
        /// </summary>
        [JsonPropertyName("index")]
        public int Index { get; set; }
        /// <summary>
        /// If this is the last segment of the completion result, this specifies why the completion has ended.
        /// </summary>
        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }
}
