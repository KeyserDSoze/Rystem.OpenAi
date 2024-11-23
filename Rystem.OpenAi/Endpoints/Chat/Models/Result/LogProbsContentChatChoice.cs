using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class LogProbsContentChatChoice : TopLogProbsContentChatChoice
    {
        /// <summary>
        /// List of the most likely tokens and their log probability, at this token position. In rare cases, there may be fewer than the number of requested top_logprobs returned.
        /// </summary>
        [JsonPropertyName("top_logprobs")]
        public List<TopLogProbsContentChatChoice>? TopLogProbs { get; set; }

    }
}
