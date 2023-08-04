using System;
using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi.Test.Logger
{
    internal class CustomLogger<T> : ILogger<T>
    {
        private readonly CustomLoggerMemory _customLoggerMemory;
        public CustomLogger(CustomLoggerMemory customLoggerMemory)
        {
            _customLoggerMemory = customLoggerMemory;
        }
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _customLoggerMemory.Logs.Add(new CustomLog
            {
                EventId = eventId,
                LogLevel = logLevel,
                Message = formatter(state, exception),
                Objects = null
            });
        }
    }
}
