using System;
using System.Net.Http;

namespace Rystem.OpenAi
{
    public abstract class RequestBuilder<T>
        where T : class, IOpenAiRequest
    {
        private protected readonly HttpClient _client;
        private protected readonly OpenAiConfiguration _configuration;
        private protected readonly T _request;
        private protected RequestBuilder(HttpClient client, OpenAiConfiguration configuration, Func<T> requestCreator)
        {
            _client = client;
            _configuration = configuration;
            _request = requestCreator.Invoke();
        }
    }
}
