using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.FineTune;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiModel : OpenAiBase, IOpenAiModel
    {
        private readonly bool _forced;
        public OpenAiModel(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
            _forced = false;
        }
        public ValueTask<Model> RetrieveAsync(string id, CancellationToken cancellationToken = default)
            => Client.GetAsync<Model>(_configuration.GetUri(OpenAiType.Model, string.Empty, _forced, $"/{id}"), _configuration, cancellationToken);
        public async Task<OpenAiList<Model>> ListAsync(CancellationToken cancellationToken = default)
        {
            var response = await Client.GetAsync<OpenAiList<Model>>(_configuration.GetUri(OpenAiType.Model, string.Empty, _forced, string.Empty), _configuration, cancellationToken);
            return response;
        }
        public ValueTask<FineTuningDeleteResult> DeleteAsync(string fineTuneId, CancellationToken cancellationToken = default)
           => Client.DeleteAsync<FineTuningDeleteResult>(_configuration.GetUri(OpenAiType.Model, fineTuneId, _forced, $"/{fineTuneId}"), _configuration, cancellationToken);
    }
}
