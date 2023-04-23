using System;
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
        public DeploymentBuilder Deployment()
        {
            if (!Configuration.WithAzure)
                throw new MethodAccessException("This method is valid only for Azure integration. Only Azure OpenAi has Deployment logic.");
            return new DeploymentBuilder(Client, Configuration, Utility);
        }
    }
}
