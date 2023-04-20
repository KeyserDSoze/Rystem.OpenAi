using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Rystem.OpenAi
{
    internal abstract class OpenAiBase
    {
        private protected readonly HttpClient Client;
        private protected OpenAiConfiguration Configuration;
        private readonly IEnumerable<OpenAiConfiguration> _configurations;
        private protected readonly IOpenAiUtility Utility;
        private protected OpenAiBase(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
        {
            Client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            Configuration = configurations.First();
            _configurations = configurations;
            Utility = utility;
        }
        public void SetName(string? name)
          => Configuration = _configurations.FirstOrDefault(x => x.Name == name);
    }
}
