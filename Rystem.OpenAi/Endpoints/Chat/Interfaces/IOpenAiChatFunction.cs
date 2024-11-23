using System;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Chat
{
    public interface IOpenAiChatFunction
    {
        string Name { get; }
        string Description { get; }
        Type Input { get; }
        Task<object> WrapAsync(string message);
    }
}
