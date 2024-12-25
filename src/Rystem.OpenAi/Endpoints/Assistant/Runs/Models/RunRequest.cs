using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    public enum RunStatus
    {
        Queued,
        InProgress,
        RequiresAction,
        Cancelling,
        Cancelled,
        Failed,
        Completed,
        Incomplete,
        Expired
    }
    internal static class RunStatusExtensions
    {
        public static RunStatus? ToRunStatus(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            return input switch
            {
                "queued" => RunStatus.Queued,
                "in_progress" => RunStatus.InProgress,
                "requires_action" => RunStatus.RequiresAction,
                "cancelling" => RunStatus.Cancelling,
                "cancelled" => RunStatus.Cancelled,
                "failed" => RunStatus.Failed,
                "completed" => RunStatus.Completed,
                "incomplete" => RunStatus.Incomplete,
                "expired" => RunStatus.Expired,
                _ => null
            };
        }
    }
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
        public int StartedAt { get; set; }
        [JsonPropertyName("expires_at")]
        public int ExpiresAt { get; set; }
        [JsonPropertyName("cancelled_at")]
        public int CancelledAt { get; set; }
        [JsonPropertyName("failed_at")]
        public int FailedAt { get; set; }
        [JsonPropertyName("completed_at")]
        public int CompletedAt { get; set; }
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
    public enum LastErrorCode
    {
        ServerError,
        RateLimitExceeded,
        InvalidPrompt
    }
    internal static class LastErrorCodeExtensions
    {
        public static LastErrorCode? ToLastErrorCode(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            return input switch
            {
                "server_error" => LastErrorCode.ServerError,
                "rate_limit_exceeded" => LastErrorCode.RateLimitExceeded,
                "invalid_prompt" => LastErrorCode.InvalidPrompt,
                _ => null
            };
        }
    }
    public sealed class LastErrorRun
    {
        [JsonPropertyName("code")]
        public string? CodeAsString { get; set; }
        [JsonIgnore]
        public LastErrorCode? Code => LastErrorCodeExtensions.ToLastErrorCode(CodeAsString);
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
    public sealed class IncompleteReasonRun
    {
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
    public sealed class RunRequest : AssistantRequest
    {
        [JsonIgnore]
        public StringBuilder? AdditionalInstructionsBuilder { get; set; }
        [JsonPropertyName("additional_instructions")]
        public string? AdditionalInstructions { get => AdditionalInstructionsBuilder?.ToString(); set => AdditionalInstructionsBuilder = new(value); }
        [JsonPropertyName("additional_messages")]
        public List<ThreadMessage>? AdditionalMessages { get; set; }
        [JsonPropertyName("thread")]
        public ThreadRequest? Thread { get; set; }
        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
        [JsonPropertyName("max_prompt_tokens")]
        public int? MaxPromptTokens { get; set; }
        [JsonPropertyName("max_completion_tokens")]
        public int? MaxCompletionTokens { get; set; }
        [JsonPropertyName("truncation_strategy")]
        public RunTruncationStrategy? RunTruncationStrategy { get; set; }
        [JsonPropertyName("tool_choice")]
        public AnyOf<string, ForcedFunctionTool>? ToolChoice { get; set; }
        [JsonPropertyName("parallel_tool_calls")]
        public bool? ParallelToolCalls { get; set; }
    }
}
