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
        private protected readonly IOpenAiUtility _utility;
        private protected OpenAiBase(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
        {
            _client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            _configuration = configurations.First();
            _configurations = configurations;
            _utility = utility;
        }
        public void SetName(string? name)
          => _configuration = _configurations.FirstOrDefault(x => x.Name == name);
    }
}
