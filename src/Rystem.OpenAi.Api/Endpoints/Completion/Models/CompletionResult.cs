using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Completion
{
    /// <summary>
    /// Represents a result from calling the Completion API
    /// </summary>
    public class CompletionResult : ApiBaseResponse
    {
        /// <summary>
        /// The identifier of the result, which may be used during troubleshooting
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        /// <summary>
        /// The completions returned by the API.  Depending on your request, there may be 1 or many choices.
        /// </summary>
        [JsonPropertyName("choices")]
        public List<CompletionChoice>? Completions { get; set; }
        /// <summary>
        /// API token usage as reported by the OpenAI API for this request
        /// </summary>
        [JsonPropertyName("usage")]
        public CompletionUsage? Usage { get; set; }
        /// <summary>
        /// Gets the text of the first completion, representing the main result
        /// </summary>
        public override string? ToString()
        {
            if (Completions != null && Completions.Count > 0)
                return Completions[0].ToString();
            else
                return $"CompletionResult {Id} has no valid output";
        }
    }
}
