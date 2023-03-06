using System.Net.Http;

namespace Rystem.OpenAi.Edit
{
    internal sealed class OpenAiEditApi : IOpenAiEditApi
    {
        private readonly HttpClient _client;
        private readonly OpenAiConfiguration _configuration;
        public OpenAiEditApi(IHttpClientFactory httpClientFactory, OpenAiConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            _configuration = configuration;
        }
        public EditRequestBuilder Request(string instruction)
            => new EditRequestBuilder(_client, _configuration, instruction);
    }
}
