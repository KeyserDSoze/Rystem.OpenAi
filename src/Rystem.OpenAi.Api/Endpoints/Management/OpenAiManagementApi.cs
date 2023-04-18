using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Management
{
    internal sealed class OpenAiManagementApi : OpenAiBase, IOpenAiManagementApi
    {
        public OpenAiManagementApi(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public BillingBuilder Billing()
            => new BillingBuilder(_client, _configuration, _utility);
    }
}
