using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Chat
{
    internal sealed class OpenAiChat : OpenAiBase, IOpenAiChat
    {
        private readonly IEnumerable<IOpenAiChatFunction> _functions;

        public OpenAiChat(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility, IEnumerable<IOpenAiChatFunction> functions)
            : base(httpClientFactory, configurations, utility)
        {
            _functions = functions;
        }
        public ChatRequestBuilder Request(ChatMessage message)
            => new ChatRequestBuilder(Client, _configuration, message, Utility, _functions);
        public ChatRequestBuilder RequestWithUserMessage(string message)
            => new ChatRequestBuilder(Client, _configuration, new ChatMessage
            {
                Content = message,
                Role = ChatRole.User
            }, Utility, _functions);
        public ChatRequestBuilder RequestWithSystemMessage(string message)
            => new ChatRequestBuilder(Client, _configuration, new ChatMessage
            {
                Content = message,
                Role = ChatRole.System
            }, Utility, _functions);
        public ChatRequestBuilder RequestWithAssistantMessage(string message)
            => new ChatRequestBuilder(Client, _configuration, new ChatMessage
            {
                Content = message,
                Role = ChatRole.Assistant
            }, Utility, _functions);
        public ChatRequestBuilder RequestWithFunctionMessage(string name, string message)
            => new ChatRequestBuilder(Client, _configuration, new ChatMessage
            {
                Name = name,
                Content = message,
                Role = ChatRole.Function
            }, Utility, _functions);
    }
}
