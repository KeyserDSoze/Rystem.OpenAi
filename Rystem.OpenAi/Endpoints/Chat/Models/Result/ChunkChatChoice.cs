using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    /// <summary>
    /// Represents a completion choice returned by the Chat API.  
    /// </summary>
    public sealed class ChunkChatChoice : BaseChatChoice
    {
        /// <summary>
        /// A part of a message.
        /// </summary>
        [JsonPropertyName("delta")]
        public ChatMessage? Delta { get; set; }
    }
}
