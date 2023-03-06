using System.Net.Http;

namespace Rystem.OpenAi.Moderation
{
    internal sealed class OpenAiModerationApi : IOpenAiModerationApi
    {
        private readonly HttpClient _client;
        private readonly OpenAiConfiguration _configuration;
        public OpenAiModerationApi(IHttpClientFactory httpClientFactory, OpenAiConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            _configuration = configuration;
        }
        public ModerationRequestBuilder Create(string input)
            => new ModerationRequestBuilder(_client, _configuration, input);
    }
}
