using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi
{
    public sealed class OpenAiLoggingConfiguration
    {
        public bool TurnOff { get; set; }
        public LogLevel Request { get; set; } = LogLevel.Debug;
        public LogLevel Error { get; set; } = LogLevel.Error;
    }
}
