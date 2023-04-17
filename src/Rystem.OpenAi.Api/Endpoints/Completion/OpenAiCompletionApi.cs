using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Completion
{
    internal sealed class OpenAiCompletionApi : OpenAiBase, IOpenAiCompletionApi
    {
        public OpenAiCompletionApi(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public CompletionRequestBuilder Request(params string[] prompts)
            => new CompletionRequestBuilder(_client, _configuration, prompts, _utility);
    }
}
