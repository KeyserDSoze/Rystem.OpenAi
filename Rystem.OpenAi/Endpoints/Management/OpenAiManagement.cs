using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi.Management
{
    internal sealed class OpenAiManagement : OpenAiBase, IOpenAiManagement
    {
        public OpenAiManagement(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiBilling openAiBilling,
            IOpenAiDeployment openAiDeployment,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
            Billing = openAiBilling;
            _deployment = openAiDeployment;
            SetAiBase(Billing);
            SetAiBase(_deployment);
        }
        public IOpenAiBilling Billing { get; }
        private readonly IOpenAiDeployment _deployment;
        public IOpenAiDeployment Deployment
        {
            get
            {
                if (!_configuration.WithAzure)
                    throw new NotImplementedException("This method is valid only for Azure integration. Only Azure OpenAi has Deployment logic.");
                return _deployment;
            }
        }
    }
}
