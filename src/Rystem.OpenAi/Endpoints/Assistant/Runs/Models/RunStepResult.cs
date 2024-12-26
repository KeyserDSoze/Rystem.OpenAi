using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepResult : ApiBaseResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
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
        [JsonPropertyName("expired_at")]
        public int ExpiredAt { get; set; }
        [JsonPropertyName("cancelled_at")]
        public int CancelledAt { get; set; }
        [JsonPropertyName("failed_at")]
        public int FailedAt { get; set; }
        [JsonPropertyName("completed_at")]
        public int CompletedAt { get; set; }
        [JsonPropertyName("last_error")]
        public LastErrorRun? LastError { get; set; }
        [JsonPropertyName("step_details")]
        public AnyOf<StepDetail, RunStepToolCalls>? StepDetails { get; set; }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
        [JsonPropertyName("usage")]
        public ChatUsage? Usage { get; set; }
    }
}
