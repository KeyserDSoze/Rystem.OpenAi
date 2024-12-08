using System.Net.Http;
using Polly;

namespace Rystem.OpenAi
{
    internal sealed class HttpClientWrapper
    {
        public required HttpClient Client { get; init; }
        public IAsyncPolicy<HttpResponseMessage>? Policy { get; init; }
    }
}
