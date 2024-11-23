using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    /// <summary>
    /// Represents a request to the chat API.
    /// </summary>
    public sealed class ChatRequest : IOpenAiRequest
    {
        [JsonPropertyName("messages")]
        public List<ChatMessage>? Messages { get; set; }
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        [JsonPropertyName("store")]
        public bool? Store { get; set; }
        [JsonPropertyName("metadata")]
        public object? Metadata { get; set; }
        [JsonPropertyName("frequency_penalty")]
        public double? FrequencyPenalty { get; set; }
        [JsonPropertyName("logit_bias")]
        public Dictionary<string, int>? Bias { get; set; }
        [JsonPropertyName("logprobs")]
        public bool? LogProbs { get; set; }
        [JsonPropertyName("top_logprobs")]
        public bool? TopLogProbs { get; set; }
        [JsonPropertyName("max_completion_tokens")]
        public int? MaxCompletionsToken { get; set; }
        [JsonPropertyName("n")]
        public int? NumberOfChoicesPerPrompt { get; set; }
        [JsonPropertyName("modalities")]
        public List<string>? Modalities { get; set; }
        [JsonPropertyName("prediction")]
        public PredictionChatRequest? Prediction { get; set; }
        [JsonPropertyName("audio")]
        public AudioChatRequest? Audio { get; set; }
        [JsonPropertyName("presence_penalty")]
        public double? PresencePenalty { get; set; }
        [JsonPropertyName("response_format")]
        public ResponseFormatChatRequest? ResponseFormat { get; set; }
        [JsonPropertyName("seed")]
        public int? Seed { get; set; }
        [JsonPropertyName("service_tier")]
        public string? ServiceTier { get; set; }
        [JsonPropertyName("stop")]
        public object? StopSequence { get; set; }
        [JsonPropertyName("stream")]
        public bool Stream { get; internal set; } = false;
        [JsonPropertyName("stream_options")]
        public StreamOptionsChatRequest? StreamOptions { get; set; }
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }
        [JsonPropertyName("top_p")]
        public double? TopP { get; set; }
        [JsonPropertyName("tools")]
        public List<object>? Tools { get; set; }
        [JsonPropertyName("tool_choice")]
        public object? ToolChoice { get; set; }
        [JsonPropertyName("parallel_tool_calls")]
        public bool? ParallelToolCalls { get; set; }
        [JsonPropertyName("user")]
        public string? User { get; set; }
    }
    public sealed class ToolChoiceChatRequest
    {
        [JsonPropertyName("tool")]
        public string? Tool { get; set; }
        [JsonPropertyName("choice")]
        public string? Choice { get; set; }
    }
    public sealed class StreamOptionsChatRequest
    {
        [JsonPropertyName("include_usage")]
        public bool IncludeUsage { get; set; }
    }
    public sealed class PredictionChatRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("content")]
        public object? Content { get; set; }
    }
    public sealed class TextPredictionChatRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
    public sealed class AudioChatRequest
    {
        [JsonPropertyName("voice")]
        public string? Voice { get; set; }
        [JsonPropertyName("format")]
        public string? Format { get; set; }
    }
    public sealed class ResponseFormatChatRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("content")]
        public object? Content { get; set; }
    }
    public sealed class JsonSchemaResponseFormatChatRequest
    {
        [JsonPropertyName("type")]
        public string? Description { get; set; }
        [JsonPropertyName("schema")]
        public string? Name { get; set; }
        [JsonPropertyName("schema")]
        public object Schema { get; set; }
    }
}
