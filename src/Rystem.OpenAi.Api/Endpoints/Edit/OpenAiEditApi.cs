using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Edit
{
    internal sealed class OpenAiEditApi : OpenAiBase, IOpenAiEditApi
    {
        public OpenAiEditApi(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations, IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public EditRequestBuilder Request(string instruction)
            => new EditRequestBuilder(_client, _configuration, instruction, _utility);
    }
}
