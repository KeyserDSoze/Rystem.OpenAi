using System;
using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiLogger : IOpenAiLogger
    {
        private readonly OpenAiLoggingConfiguration _configuration;
        private readonly ILogger<IOpenAi>? _logger;
        private readonly LogBringer _logBringer;
        public OpenAiLogger(OpenAiLoggingConfiguration configuration, ILogger<IOpenAi>? logger = null)
        {
            _configuration = configuration;
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
            if (!_configuration.TurnOff)
                _logger?.Log(_configuration.Request, "{LogBringer}", _logBringer.ToString());
        }
        public void LogError()
        {
            _logBringer.EndTime = DateTimeOffset.UtcNow;
            if (!_configuration.TurnOff)
                _logger?.Log(_configuration.Error, "{LogBringer}", _logBringer.ToString());
        }
        public override string ToString()
            => _logBringer.ToString();
    }
}
