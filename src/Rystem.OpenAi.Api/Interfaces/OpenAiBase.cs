using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi
{
    internal abstract class OpenAiBase
    {
        private protected readonly HttpClient Client;
        private protected OpenAiConfiguration _configuration;
        private readonly IEnumerable<OpenAiConfiguration> _configurations;
        private protected readonly IOpenAiUtility Utility;
        private readonly List<OpenAiBase> _openAiBases = new List<OpenAiBase>();
        private protected OpenAiBase(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
        {
            Client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            _configuration = configurations.First();
            _configurations = configurations;
            Utility = utility;
        }
        private protected void SetAiBase<T>(T entity)
        {
            if (entity is OpenAiBase aiBase)
                _openAiBases.Add(aiBase);
        }
        public void SetName(string? name)
        {
            _configuration = _configurations.FirstOrDefault(x => x.Name == name);
            foreach (var bases in _openAiBases)
                bases.SetName(name);
        }
    }
}
