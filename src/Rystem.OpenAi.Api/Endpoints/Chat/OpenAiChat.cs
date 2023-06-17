using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Chat
{
    internal sealed class OpenAiChat : OpenAiBase, IOpenAiChat
    {
        public OpenAiChat(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public ChatRequestBuilder Request(ChatMessage message)
            => new ChatRequestBuilder(Client, _configuration, message, Utility);
        public ChatRequestBuilder RequestWithUserMessage(string message)
            => new ChatRequestBuilder(Client, _configuration, new ChatMessage
            {
                Content = message,
                Role = ChatRole.User
            }, Utility);
        public ChatRequestBuilder RequestWithSystemMessage(string message)
            => new ChatRequestBuilder(Client, _configuration, new ChatMessage
            {
                Content = message,
                Role = ChatRole.System
            }, Utility);
        public ChatRequestBuilder RequestWithAssistantMessage(string message)
            => new ChatRequestBuilder(Client, _configuration, new ChatMessage
            {
                Content = message,
                Role = ChatRole.Assistant
            }, Utility);
        public ChatRequestBuilder RequestWithFunctionMessage(string name, string message)
            => new ChatRequestBuilder(Client, _configuration, new ChatMessage
            {
                Name = name,
                Content = message,
                Role = ChatRole.Function
            }, Utility);
    }
}
