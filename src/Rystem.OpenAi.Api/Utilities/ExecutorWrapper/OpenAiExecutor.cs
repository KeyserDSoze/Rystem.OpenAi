using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiExecutor : IOpenAiExecutor
    {
        public ILogger<IOpenAiExecutor>? Logger { get; }
        private static readonly object? s_defaultObject = default;
        public OpenAiExecutor(ILogger<IOpenAiExecutor>? logger = null)
        {
            Logger = logger;
        }
        private static readonly EventId s_eventId = new EventId(198_754, "Rystem.OpenAi");

        public Task ExecuteAsync(string message, Func<Task> actionToExecute)
            => ExecuteAsync(message, async () =>
                {
                    await actionToExecute().ConfigureAwait(false);
                    return s_defaultObject;
                });
        public async Task<T> ExecuteAsync<T>(string message, Func<Task<T>> actionToExecute)
        {
            if (Logger != null)
            {
                var timer = new Stopwatch();
                timer.Start();
                Logger.LogInformation(s_eventId, $"{message} starts.");
                try
                {
                    var result = await actionToExecute().ConfigureAwait(false);
                    timer.Stop();
                    Logger.LogInformation(s_eventId, $"{message} ends in {timer.ElapsedMilliseconds}ms.");
                    return result;
                }
                catch (Exception ex)
                {
                    timer.Stop();
                    Logger.LogError(s_eventId, ex.Message, $"{message} in error after {timer.ElapsedMilliseconds}ms.");
                    throw;
                }
            }
            else
                return await actionToExecute().ConfigureAwait(false);
        }
        public void Execute(string message, Action actionToExecute)
        {
            _ = ExecuteAsync(message, () =>
            {
                actionToExecute();
                return Task.FromResult(s_defaultObject);
            });
        }
        public T Execute<T>(string message, Func<T> actionToExecute)
            => ExecuteAsync(message, () =>
                {
                    var result = actionToExecute();
                    return Task.FromResult(result);
                }).Result;
    }
}
