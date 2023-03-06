using System.Net.Http;

namespace Rystem.OpenAi.Chat
{
    internal sealed class OpenAiChatApi : IOpenAiChatApi
    {
        private readonly HttpClient _client;
        private readonly OpenAiConfiguration _configuration;
        public OpenAiChatApi(IHttpClientFactory httpClientFactory, OpenAiConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            _configuration = configuration;
        }
        public ChatRequestBuilder Request(ChatMessage message)
            => new ChatRequestBuilder(_client, _configuration, message);
    }
}
