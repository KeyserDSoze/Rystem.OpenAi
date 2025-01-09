using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
    internal sealed class OpenAiLogger : IOpenAiLogger
    {
        private readonly ILogger<IOpenAi>? _logger;
        private readonly LogBringer _logBringer;
        public OpenAiLogger(ILogger<IOpenAi>? logger = null)
        {
            _logger = logger;
            _logBringer = new();
        }
        public IOpenAiLogger CreateId()
        {
            _logBringer.RequestId = Guid.NewGuid().ToString();
            return this;
        }
        public IOpenAiLogger ConfigureId(string id)
        {
            _logBringer.RequestId = id;
            return this;
        }
        public IOpenAiLogger ConfigureFactory(string? factoryName)
        {
            _logBringer.FactoryName = factoryName;
            return this;
        }
        public IOpenAiLogger ConfigureTypes(OpenAiType[] types)
        {
            _logBringer.Types = types;
            return this;
        }
        public IOpenAiLogger ConfigureUrl(string endpoint)
        {
            _logBringer.Url = endpoint;
            return this;
        }
        public IOpenAiLogger ConfigureMethod(string method)
        {
            _logBringer.Method = method;
            return this;
        }
        public IOpenAiLogger WithStreaming()
        {
            _logBringer.Streaming = true;
            return this;
        }
        public IOpenAiLogger WithoutStreaming()
        {
            _logBringer.Streaming = false;
            return this;
        }
        public IOpenAiLogger AddContent(object? content)
        {
            _logBringer.Content = content;
            return this;
        }
        public IOpenAiLogger AddResponse(object? response)
        {
            _logBringer.Response = response;
            return this;
        }
        public IOpenAiLogger AddException(Exception exception)
        {
            _logBringer.Error = exception.Message;
            return this;
        }
        public IOpenAiLogger AddError(string error)
        {
            _logBringer.Error = error;
            return this;
        }
        public IOpenAiLogger Count()
        {
            _logBringer.Count++;
            return this;
        }
        public IOpenAiLogger StartTimer()
        {
            _logBringer.StartTime = DateTimeOffset.UtcNow;
            return this;
        }
        public void LogInformation()
        {
            _logBringer.EndTime = DateTimeOffset.UtcNow;
            _logger?.LogInformation("{LogBringer}", _logBringer.ToString());
        }
        public void LogError()
        {
            _logBringer.EndTime = DateTimeOffset.UtcNow;
            _logger?.LogError("{LogBringer}", _logBringer.ToString());
        }
        public void LogWarning()
        {
            _logBringer.EndTime = DateTimeOffset.UtcNow;
            _logger?.LogWarning("{LogBringer}", _logBringer.ToString());
        }
        public void LogDebug()
        {
            _logBringer.EndTime = DateTimeOffset.UtcNow;
            _logger?.LogDebug("{LogBringer}", _logBringer.ToString());
        }
        public void LogTrace()
        {
            _logBringer.EndTime = DateTimeOffset.UtcNow;
            _logger?.LogTrace("{LogBringer}", _logBringer.ToString());
        }
        public override string ToString()
            => _logBringer.ToString();
    }
}
