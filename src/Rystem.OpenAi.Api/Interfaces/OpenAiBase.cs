using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Rystem.OpenAi
{
    internal abstract class OpenAiBase
    {
        private protected readonly HttpClient _client;
        private protected OpenAiConfiguration _configuration;
        private readonly IEnumerable<OpenAiConfiguration> _configurations;
        public OpenAiBase(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations)
        {
            _client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            _configuration = configurations.First();
            _configurations = configurations;
        }
        public void SetName(string? name)
          => _configuration = _configurations.FirstOrDefault(x => x.Name == name);
    }
}
