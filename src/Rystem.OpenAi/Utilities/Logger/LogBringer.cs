using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    internal sealed class LogBringer
    {
        [JsonPropertyName("id")]
        public string? RequestId { get; set; }
        [JsonPropertyName("type")]
        public OpenAiType[]? Types { get; set; }
        [JsonPropertyName("error")]
        public string? Error { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
        [JsonPropertyName("method")]
        public string? Method { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("streaming")]
        public bool? Streaming { get; set; }
        [JsonPropertyName("content")]
        public object? Content { get; set; }
        [JsonPropertyName("response")]
        public object? Response { get; set; }
        [JsonPropertyName("startTime")]
        public DateTimeOffset StartTime { get; set; }
        [JsonPropertyName("endTime")]
        public DateTimeOffset EndTime { get; set; }
        [JsonPropertyName("executionTime")]
        public double ExecutionTimeInMicroseconds => (EndTime - StartTime).TotalMicroseconds;
        [JsonPropertyName("factory")]
        public string? FactoryName { get; set; }
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine();
            if (RequestId != null)
                builder.AppendLine($"Id: {RequestId}");
            if (Types != null)
                builder.AppendLine($"Types: {string.Join(',', Types)}");
            if (FactoryName != null)
                builder.AppendLine($"Factory: {FactoryName}");
            if (Error != null)
                builder.AppendLine($"Error: {Error}");
            if (Url != null)
                builder.AppendLine($"Url: {Url}");
            if (Method != null)
                builder.AppendLine($"Method: {Method}");
            if (Streaming != null)
                builder.AppendLine($"Streaming: {Streaming}");
            if (Streaming == true)
                builder.AppendLine($"Stream count: {Count}");
            if (StartTime != default)
                builder.AppendLine($"Start Time: {StartTime}");
            if (EndTime != default)
                builder.AppendLine($"End Time: {EndTime}");
            if (StartTime != default && EndTime != default)
                builder.AppendLine($"Execution Time: {ExecutionTimeInMicroseconds} microseconds");
            if (Content != null)
                builder.AppendLine($"Content: {(Content is string ? Content : Content?.ToJson(s_options))}");
            if (Response != null)
                builder.AppendLine($"Response: {(Response is string ? Response : Response?.ToJson(s_options))}");
            return builder.ToString();
        }
        private static readonly JsonSerializerOptions s_options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }
}
