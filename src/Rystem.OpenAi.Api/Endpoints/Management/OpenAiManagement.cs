using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Management
{
    internal sealed class OpenAiManagement : OpenAiBase, IOpenAiManagement, IOpenAiManagementApi
    {
        public OpenAiManagement(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public BillingBuilder Billing()
            => new BillingBuilder(Client, Configuration, Utility);
    }
}
