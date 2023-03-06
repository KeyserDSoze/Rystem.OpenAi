using System.Net.Http;

namespace Rystem.OpenAi.Completion
{
    internal sealed class OpenAiCompletionApi : IOpenAiCompletionApi
    {
        private readonly HttpClient _client;
        private readonly OpenAiConfiguration _configuration;
        public OpenAiCompletionApi(IHttpClientFactory httpClientFactory, OpenAiConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            _configuration = configuration;
        }
        public CompletionRequestBuilder Request(params string[] prompts)
            => new CompletionRequestBuilder(_client, _configuration, prompts);
    }
}
