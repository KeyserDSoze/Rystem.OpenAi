using System.Net.Http;

namespace Rystem.OpenAi.Embedding
{
    internal sealed class OpenAiEmbeddingApi : IOpenAiEmbeddingApi
    {
        private readonly HttpClient _client;
        private readonly OpenAiConfiguration _configuration;
        public OpenAiEmbeddingApi(IHttpClientFactory httpClientFactory, OpenAiConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            _configuration = configuration;
        }
        public EmbeddingRequestBuilder Request(params string[] inputs)
            => new EmbeddingRequestBuilder(_client, _configuration, inputs);
    }
}
