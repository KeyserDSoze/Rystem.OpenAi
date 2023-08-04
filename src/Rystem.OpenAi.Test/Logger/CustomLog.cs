using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi.Test.Logger
{
    public sealed class CustomLog
    {
        public EventId EventId { get; set; }
        public string Message { get; set; }
        public LogLevel LogLevel { get; set; }
        public object[]? Objects { get; set; }
    }
}
