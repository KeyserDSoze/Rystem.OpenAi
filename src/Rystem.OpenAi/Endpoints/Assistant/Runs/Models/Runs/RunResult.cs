using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    [AnyOfJsonDefault]
    public sealed class RunResult : ApiBaseResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("assistant_id")]
        public string? AssistantId { get; set; }
        [JsonPropertyName("thread_id")]
        public string? ThreadId { get; set; }
        [JsonPropertyName("status")]
        public string? StatusAsString { get; set; }
        [JsonIgnore]
        public RunStatus? Status => RunStatusExtensions.ToRunStatus(StatusAsString);
        [JsonPropertyName("started_at")]
        public int? StartedAt { get; set; }
        [JsonPropertyName("expires_at")]
        public int? ExpiresAt { get; set; }
        [JsonPropertyName("cancelled_at")]
        public int? CancelledAt { get; set; }
        [JsonPropertyName("failed_at")]
        public int? FailedAt { get; set; }
        [JsonPropertyName("completed_at")]
        public int? CompletedAt { get; set; }
        [JsonPropertyName("last_error")]
        public LastErrorRun? LastError { get; set; }
        [JsonPropertyName("instructions")]
        public string? Instructions { get; set; }
        [JsonPropertyName("incomplete_details")]
        public IncompleteReasonRun? IncompleteDetails { get; set; }
        [JsonPropertyName("tools")]
        public List<AnyOf<AssistantFunctionTool, AssistantCodeInterpreterTool, AssistantFileSearchTool>>? Tools { get; set; }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
        [JsonPropertyName("usage")]
        public ChatUsage? Usage { get; set; }
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }
        [JsonPropertyName("top_p")]
        public double? TopP { get; set; }
        [JsonPropertyName("max_prompt_tokens")]
        public int? MaxPromptTokens { get; set; }
        [JsonPropertyName("max_completion_tokens")]
        public int? MaxCompletionTokens { get; set; }
        [JsonPropertyName("truncation_strategy")]
        public RunTruncationStrategy? TruncationStrategy { get; set; }
        [JsonPropertyName("response_format")]
        public AnyOf<string, ResponseFormatChatRequest>? ResponseFormat { get; set; }
        [JsonPropertyName("tool_choice")]
        public AnyOf<string, ForcedFunctionTool>? ToolChoice { get; set; }
        [JsonPropertyName("parallel_tool_calls")]
        public bool ParallelToolCalls { get; set; }
    }
}
