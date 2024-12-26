using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
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
