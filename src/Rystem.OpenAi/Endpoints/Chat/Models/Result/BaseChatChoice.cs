using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public abstract class BaseChatChoice
    {
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
        /// <summary>
        /// Log probability information for the choice.
        /// </summary>
        [JsonPropertyName("logprobs")]
        public LogProbsChatChoice? LogProbs { get; set; }
    }
}
