using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepToolCalls
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("tool_calls")]
        public List<RunStepToolCall>? Tools { get; set; }
    }
    //public sealed class RunStepFunctionTool
    //{
    //    [JsonPropertyName("id")]
    //    public string? Id { get; set; }
    //    [JsonPropertyName("type")]
    //    public string? Type { get; set; }
    //    [JsonPropertyName("file_search")]
    //    public Dictionary<RunStepRankingOptionFileSearchTool, List<RunStepRankingResultFileSearchTool>>? FileSearch { get; set; }
    //}
    public sealed class RunStepFileSearchTool
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("file_search")]
        public Dictionary<RunStepRankingOptionFileSearchTool, List<RunStepRankingResultFileSearchTool>>? FileSearch { get; set; }
    }
    public sealed class RunStepRankingOptionFileSearchTool
    {
        [JsonPropertyName("ranker")]
        public string? Ranker { get; set; }
        [JsonPropertyName("score_threshold")]
        public float? ScoreThreshold { get; set; }
    }
    public sealed class RunStepRankingResultFileSearchTool
    {
        [JsonPropertyName("file_id")]
        public string? FileId { get; set; }
        [JsonPropertyName("file_name")]
        public string? FileName { get; set; }
        [JsonPropertyName("score")]
        public float? Score { get; set; }
        [JsonPropertyName("content")]
        public RunStepRankingResultContentFileSearchTool? Content { get; set; }
    }
    public sealed class RunStepRankingResultContentFileSearchTool
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
    public sealed class RunStepCodeInterpreterTool
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("code_interpreter")]
        public RunStepCodeInterpreterObjectTool? CodeInterpreter { get; set; }
    }
    public sealed class RunStepCodeInterpreterObjectTool
    {
        [JsonPropertyName("input")]
        public string? Input { get; set; }
        [JsonPropertyName("outputs")]
        public List<AnyOf<RunStepCodeInterpreterObjectLogsTool, RunStepCodeInterpreterObjectImageTool>>? Outputs { get; set; }
    }
    public sealed class RunStepCodeInterpreterObjectLogsTool
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("logs")]
        public string? Logs { get; set; }
    }
    public sealed class RunStepCodeInterpreterObjectImageTool
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("image")]
        public RunStepCodeInterpreterObjectImageObjectTool? Image { get; set; }
    }
    public sealed class RunStepCodeInterpreterObjectImageObjectTool
    {
        [JsonPropertyName("file_id")]
        public string? FileId { get; set; }
    }
}
