using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi.Api.Utilities.Logs
{
    internal sealed class OpenAiLogger : IOpenAiLogger
    {
        public ILogger<IOpenAiLogger> Custom { get; }
        public OpenAiLogger(ILogger<IOpenAiLogger> logger)
        {
            Custom = logger;
        }
        public async Task LogAsync(Func<Task> toExecuteWithLog, string message)
        {
            var timer = new Stopwatch();
            timer.Start();
            Custom.LogInformation($"{message} starts.");
            try
            {
                await toExecuteWithLog().ConfigureAwait(false);
                timer.Stop();
                Custom.LogInformation($"{message} ends.", timer);
            }
            catch (Exception ex)
            {
                timer.Stop();
                Custom.LogError(ex.Message, $"{message} in error.", timer);
                throw;
            }
        }
        public void Log(Action toExecuteWithLog, string message)
        {
            var timer = new Stopwatch();
            timer.Start();
            Custom.LogInformation($"{message} starts.");
            try
            {
                toExecuteWithLog();
                timer.Stop();
                Custom.LogInformation($"{message} ends.", timer);
            }
            catch (Exception ex)
            {
                timer.Stop();
                Custom.LogError(ex.Message, $"{message} in error.", timer);
                throw;
            }
        }
    }
}
