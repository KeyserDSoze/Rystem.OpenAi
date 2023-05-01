using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Management
{
    internal sealed class OpenAiBilling : OpenAiBase, IOpenAiBilling
    {
        public OpenAiBilling(IHttpClientFactory httpClientFactory,
          IEnumerable<OpenAiConfiguration> configurations,
          IOpenAiUtility utility)
          : base(httpClientFactory, configurations, utility)
        {
        }
        public BillingBuilder From(DateTime? from = null)
            => new BillingBuilder(Client, _configuration, Utility, from);
    }
}
