using System.Text.Json.Serialization;

namespace Rystem.PlayFramework
{
    public sealed class AiSceneResponse
    {
        [JsonPropertyName("rk")]
        public required string RequestKey { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("functionName")]
        public string? FunctionName { get; set; }
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("args")]
        public object? Arguments { get; set; }
        [JsonPropertyName("response")]
        public string? Response { get; set; }
        [JsonPropertyName("status")]
        public required AiResponseStatus Status { get; set; }
        [JsonPropertyName("responseTime")]
        public required DateTime ResponseTime { get; set; }
        
        /// <summary>
        /// Cost of this specific OpenAI request (if applicable)
        /// </summary>
        [JsonPropertyName("cost")]
        public decimal? Cost { get; set; }
        
        /// <summary>
        /// Total accumulated cost for the entire conversation
        /// </summary>
        [JsonPropertyName("totalCost")]
        public decimal? TotalCost { get; set; }

        /// <summary>
        /// Number of input tokens used in this request
        /// </summary>
        [JsonPropertyName("inputTokens")]
        public int? InputTokens { get; set; }

        /// <summary>
        /// Number of cached input tokens reused in this request
        /// Cached tokens reduce cost and improve performance
        /// </summary>
        [JsonPropertyName("cachedInputTokens")]
        public int? CachedInputTokens { get; set; }

        /// <summary>
        /// Number of output tokens generated in this request
        /// </summary>
        [JsonPropertyName("outputTokens")]
        public int? OutputTokens { get; set; }

        /// <summary>
        /// Total tokens used in this request (input + output)
        /// </summary>
        [JsonPropertyName("totalTokens")]
        public int? TotalTokens { get; set; }

        /// <summary>
        /// Name of the model used for this response
        /// </summary>
        [JsonPropertyName("model")]
        public string? Model { get; set; }
    }
}
