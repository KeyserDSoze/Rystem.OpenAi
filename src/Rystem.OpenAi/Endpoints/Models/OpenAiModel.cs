using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Models
{
    internal sealed class OpenAiModel : OpenAiBuilder<IOpenAiModel>, IOpenAiModel
    {
        public OpenAiModel(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLoggerFactory loggerFactory)
            : base(factory, configurationFactory, loggerFactory, OpenAiType.Model)
        {
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.Model != null)
            {
                configuration.Settings.DefaultRequestConfiguration.Model.Invoke(this);
            }
        }
        public ValueTask<ModelResult> RetrieveAsync(string id, CancellationToken cancellationToken = default)
            => DefaultServices.HttpClientWrapper
                .GetAsync<ModelResult>(
                    DefaultServices.Configuration.GetUri(OpenAiType.Model, _version, string.Empty, $"/{id}", null),
                    null,
                    DefaultServices.Configuration,
                    LoggerFactory.Create(),
                    cancellationToken);
        public async Task<ResponseAsArray<ModelResult>> ListAsync(CancellationToken cancellationToken = default)
        {
            var response = await DefaultServices.HttpClientWrapper
                            .GetAsync<ResponseAsArray<ModelResult>>(
                                DefaultServices.Configuration.GetUri(OpenAiType.Model, _version, string.Empty, string.Empty, null),
                                null,
                                DefaultServices.Configuration,
                                LoggerFactory.Create(),
                                cancellationToken);
            return response;
        }
        public ValueTask<DeleteResponse> DeleteAsync(string fineTuneId, CancellationToken cancellationToken = default)
           => DefaultServices.HttpClientWrapper
                .DeleteAsync<DeleteResponse>(
                    DefaultServices.Configuration.GetUri(OpenAiType.Model, _version, fineTuneId, $"/{fineTuneId}", null),
                    null,
                    DefaultServices.Configuration,
                    LoggerFactory.Create(),
                    cancellationToken);
    }
}
