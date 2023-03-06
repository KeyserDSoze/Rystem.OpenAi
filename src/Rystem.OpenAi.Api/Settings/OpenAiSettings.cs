using System.Net.Http;
using Polly;

namespace Rystem.OpenAi
{
    public sealed class OpenAiSettings
    {
        public string? ApiKey { get; set; }
        public string? OrganizationName { get; set; }
        public string? Version { get; set; }
        public OpenAiAzureSettings Azure { get; } = new OpenAiAzureSettings();
        public bool RetryPolicy { get; set; } = true;
        public IAsyncPolicy<HttpResponseMessage>? CustomRetryPolicy { get; set; }
        internal const string HttpClientName = "openaiclient_rystem";
    }
}
