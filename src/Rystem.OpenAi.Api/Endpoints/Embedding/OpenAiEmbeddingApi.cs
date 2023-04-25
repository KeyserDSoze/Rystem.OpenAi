using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Embedding
{
    internal sealed class OpenAiEmbedding : OpenAiBase, IOpenAiEmbedding
    {
        public OpenAiEmbedding(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public EmbeddingRequestBuilder Request(params string[] inputs)
            => new EmbeddingRequestBuilder(Client, Configuration, inputs, Utility);
    }
}
