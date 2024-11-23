using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public class TopLogProbsContentChatChoice
    {
        /// <summary>
        /// The token.
        /// </summary>
        [JsonPropertyName("token")]
        public string? Token { get; set; }
        /// <summary>
        /// The log probability of this token, if it is within the top 20 most likely tokens. Otherwise, the value -9999.0 is used to signify that the token is very unlikely.
        /// </summary>
        [JsonPropertyName("logprob")]
        public long LogProb { get; set; }
        /// <summary>
        /// A list of integers representing the UTF-8 bytes representation of the token. Useful in instances where characters are represented by multiple tokens and their byte representations must be combined to generate the correct text representation. Can be null if there is no bytes representation for the token.
        /// </summary>
        [JsonPropertyName("bytes")]
        public float[] Bytes { get; set; }
    }
}
