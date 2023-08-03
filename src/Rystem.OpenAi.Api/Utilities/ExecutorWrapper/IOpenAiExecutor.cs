using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi
{
    public interface IOpenAiExecutor
    {
        ILogger<IOpenAiExecutor> Logger { get; }
        Task ExecuteAsync(string message, Func<Task> actionToExecute);
        Task<T> ExecuteAsync<T>(string message, Func<Task<T>> actionToExecute);
        void Execute(string message, Action actionToExecute);
        T Execute<T>(string message, Func<T> actionToExecute);
    }
}
