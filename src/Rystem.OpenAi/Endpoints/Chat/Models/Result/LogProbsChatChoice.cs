using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class LogProbsChatChoice
    {
        /// <summary>
        /// A list of message content tokens with log probability information.
        /// </summary>
        [JsonPropertyName("content")]
        public LogProbsContentChatChoice? Content { get; set; }
        /// <summary>
        /// A list of message refusal tokens with log probability information.
        /// </summary>
        [JsonPropertyName("refusal")]
        public LogProbsContentChatChoice? Refusal { get; set; }
    }
}
