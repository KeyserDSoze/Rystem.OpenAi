using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    internal sealed class DefaultServices : IServiceForFactory
    {
        private readonly IFactory<HttpClient> _httpClientFactory;
        private readonly IFactory<OpenAiConfiguration> _configurationFactory;
        private readonly IFactory<IOpenAiPriceService> _priceService;
#pragma warning disable CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.
        public DefaultServices(IFactory<HttpClient> httpClientFactory,
            IFactory<OpenAiConfiguration> configurationFactory,
            IFactory<IOpenAiPriceService> priceService,
            IOpenAiUtility utility)
        {
            _httpClientFactory = httpClientFactory;
            _configurationFactory = configurationFactory;
            _priceService = priceService;
            Utility = utility;
        }
#pragma warning restore CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.
        public HttpClient HttpClient => _httpClientFactory.Create($"{OpenAiSettings.HttpClientName}_{_factoryName}")!;
        public IOpenAiUtility Utility { get; }
        public OpenAiConfiguration Configuration => field ??= _configurationFactory.Create(_factoryName)!;
        public IOpenAiPriceService Price => field ??= _priceService.Create(_factoryName)!;
        private string? _factoryName;
        public void SetFactoryName(string name)
        {
            _factoryName = name;
        }
    }
}
