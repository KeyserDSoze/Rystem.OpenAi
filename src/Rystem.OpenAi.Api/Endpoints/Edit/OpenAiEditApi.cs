using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Edit
{
    internal sealed class OpenAiEditApi : OpenAiBase, IOpenAiEditApi
    {
        public OpenAiEditApi(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations)
            : base(httpClientFactory, configurations)
        {
        }
        public EditRequestBuilder Request(string instruction)
            => new EditRequestBuilder(_client, _configuration, instruction);
    }
}
