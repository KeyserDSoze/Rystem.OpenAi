using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Completion
{
    /// <summary>
    /// Represents a request to the Completions API.  Mostly matches the parameters in <see href="https://beta.openai.com/api-ref#create-completion-post">the OpenAI docs</see>, although some have been renames or expanded into single/multiple properties for ease of use.
    /// </summary>
    public sealed class CompletionRequest : IOpenAiRequest
    {
        [JsonPropertyName("model")]
        public string? ModelId { get; set; }
        [JsonPropertyName("prompt")]
        public object? Prompt { get; set; }
        [JsonPropertyName("suffix")]
        public string? Suffix { get; set; }
        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }
        [JsonPropertyName("top_p")]
        public double? TopP { get; set; }
        [JsonPropertyName("presence_penalty")]
        public double? PresencePenalty { get; set; }
        [JsonPropertyName("frequency_penalty")]
        public double? FrequencyPenalty { get; set; }
        [JsonPropertyName("n")]
        public int? NumberOfChoicesPerPrompt { get; set; }
        [JsonPropertyName("stream")]
        public bool Stream { get; internal set; } = false;
        [JsonPropertyName("logprobs")]
        public int? Logprobs { get; set; }
        [JsonPropertyName("echo")]
        public bool? Echo { get; set; }
        [JsonPropertyName("stop")]
        public object? StopSequence { get; set; }
        [JsonPropertyName("best_of")]
        public int? BestOf { get; set; }
        [JsonPropertyName("logit_bias")]
        public Dictionary<string, int>? Bias { get; set; }
        [JsonPropertyName("user")]
        public string? User { get; set; }
    }
}
