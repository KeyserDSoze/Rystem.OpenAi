using System.IO;
using System.Net.Http;

namespace Rystem.OpenAi.Audio
{
    internal sealed class OpenAiAudioApi : IOpenAiAudioApi
    {
        private readonly HttpClient _client;
        private readonly OpenAiConfiguration _configuration;
        public OpenAiAudioApi(IHttpClientFactory httpClientFactory, OpenAiConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            _configuration = configuration;
        }
        public AudioRequestBuilder Request(Stream file, string fileName = "default")
            => new AudioRequestBuilder(_client, _configuration, file, fileName);
    }
}
