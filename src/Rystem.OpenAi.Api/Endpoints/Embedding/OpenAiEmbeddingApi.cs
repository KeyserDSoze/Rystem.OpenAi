using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Embedding
{
    internal sealed class OpenAiEmbeddingApi : OpenAiBase, IOpenAiEmbeddingApi
    {
        public OpenAiEmbeddingApi(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public EmbeddingRequestBuilder Request(params string[] inputs)
            => new EmbeddingRequestBuilder(_client, _configuration, inputs, _utility);
    }
}
