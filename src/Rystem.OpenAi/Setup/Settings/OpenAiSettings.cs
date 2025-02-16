using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Rystem.OpenAi
{
    public sealed class OpenAiSettings : IFactoryOptions
    {
        public string? ApiKey { get; set; }
        public string? OrganizationName { get; set; }
        public string? ProjectId { get; set; }
        public string? DefaultVersion { get; set; }
        public DefaultRequestConfiguration DefaultRequestConfiguration { get; set; } = new();
        private OpenAiAzureSettings? _azureSettings;
        public OpenAiAzureSettings Azure => _azureSettings ??= new OpenAiAzureSettings();
        /// <summary>
        /// If you not set a retry policy, the default value is 3 retries every 0.5 seconds.
        /// </summary>
        public bool RetryPolicy { get; set; }
        /// <summary>
        /// Sample with 3 retries every 0.5 seconds:
        /// Policy<HttpResponseMessage>
        ///.Handle<HttpRequestException>()
        ///.OrTransientHttpError()
        ///.WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(0.5));
        /// </summary>
        public IAsyncPolicy<HttpResponseMessage>? CustomRetryPolicy { get; set; }
        /// <summary>
        /// 50% of requests fail in 10 seconds time window with a minimum of 10 requests in 15 seconds.
        /// </summary>
        /// <returns></returns>
        public OpenAiSettings SetCircuitBreakerDefaultRetryPolicy()
        {
            RetryPolicy = true;
            CustomRetryPolicy = Policy<HttpResponseMessage>
                   .Handle<HttpRequestException>()
                   .OrTransientHttpError()
                   .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromSeconds(10), 10, TimeSpan.FromSeconds(15));
            return this;
        }
        internal const string HttpClientName = "openaiclient_rystem";
        public PriceBuilder PriceBuilder { get; } = PriceBuilder.Default;
    }
}
