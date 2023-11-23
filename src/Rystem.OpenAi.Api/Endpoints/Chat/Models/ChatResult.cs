using System.Collections.Generic;
using System.Text.Json.Serialization;
using Azure.Core;

namespace Rystem.OpenAi.Chat
{
    /// <summary>
    /// Represents a result from calling the Chat API
    /// </summary>
    public class ChatResult : ApiBaseResponse
    {
        /// <summary>
        /// The identifier of the result, which may be used during troubleshooting
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        /// <summary>
        /// The chat returned by the API. Depending on your request, there may be 1 or many choices.
        /// </summary>
        [JsonPropertyName("choices")]
        public List<ChatChoice>? Choices { get; set; }
        /// <summary>
        /// This fingerprint represents the backend configuration that the model runs with.
        /// Can be used in conjunction with the seed request parameter to understand when backend changes have been made that might impact determinism.
        /// </summary>
        [JsonPropertyName("system_fingerprint")]
        public string? SystemFingerPrint { get; set; }
        /// <summary>
        /// API token usage as reported by the OpenAI API for this request
        /// </summary>
        [JsonPropertyName("usage")]
        public CompletionUsage? Usage { get; set; }
    }
}
