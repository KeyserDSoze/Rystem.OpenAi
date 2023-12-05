using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Rystem.OpenAi
{
    internal abstract class OpenAiBase
    {
        private protected HttpClient Client { get; set; }
        private protected OpenAiConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEnumerable<OpenAiConfiguration> _configurations;
        private protected readonly IOpenAiUtility Utility;
        private readonly List<OpenAiBase> _openAiBases = new List<OpenAiBase>();
        private protected OpenAiBase(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
        {
            _configuration = configurations.First();
            _httpClientFactory = httpClientFactory;
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
            Client = _httpClientFactory.CreateClient($"{OpenAiSettings.HttpClientName}-{name}");
            foreach (var bases in _openAiBases)
                bases.SetName(name);
        }
    }
}
