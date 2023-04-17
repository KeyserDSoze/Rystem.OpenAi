using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Chat
{
    internal sealed class OpenAiChatApi : OpenAiBase, IOpenAiChatApi
    {
        public OpenAiChatApi(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public ChatRequestBuilder Request(ChatMessage message)
            => new ChatRequestBuilder(_client, _configuration, message, _utility);
    }
}
