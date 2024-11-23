using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    /// <summary>
    /// Represents a completion choice returned by the Chat API.  
    /// </summary>
    public sealed class ChatChoice : BaseChatChoice
    {
        /// <summary>
        /// Messages.
        /// </summary>
        [JsonPropertyName("message")]
        public ChatMessage? Message { get; set; }

    }
}
