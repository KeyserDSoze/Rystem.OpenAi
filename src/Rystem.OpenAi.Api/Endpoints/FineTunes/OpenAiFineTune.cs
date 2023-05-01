using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.FineTune
{
    internal sealed class OpenAiFineTune : OpenAiBase, IOpenAiFineTune
    {
        private readonly bool _forced;
        public OpenAiFineTune(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations, IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
            _forced = false;
        }
        public FineTuneRequestBuilder Create(string trainingFileId)
            => new FineTuneRequestBuilder(Client, _configuration, trainingFileId, Utility);
        public ValueTask<FineTuneResults> ListAsync(CancellationToken cancellationToken = default)
            => Client.GetAsync<FineTuneResults>(_configuration.GetUri(OpenAiType.FineTune, string.Empty, _forced, string.Empty), _configuration, cancellationToken);
        public ValueTask<FineTuneResult> RetrieveAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => Client.GetAsync<FineTuneResult>(_configuration.GetUri(OpenAiType.FineTune, fineTuneId, _forced, $"/{fineTuneId}"), _configuration, cancellationToken);
        public ValueTask<FineTuneResult> CancelAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => Client.PostAsync<FineTuneResult>(_configuration.GetUri(OpenAiType.FineTune, fineTuneId, _forced, $"/{fineTuneId}/cancel"), null, _configuration, cancellationToken);
        public ValueTask<FineTuneEventsResult> ListEventsAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => Client.GetAsync<FineTuneEventsResult>(_configuration.GetUri(OpenAiType.FineTune, fineTuneId, _forced, $"/{fineTuneId}/events"), _configuration, cancellationToken);
        public ValueTask<FineTuneDeleteResult> DeleteAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => Client.DeleteAsync<FineTuneDeleteResult>(_configuration.GetUri(OpenAiType.FineTune, fineTuneId, _forced, $"/{fineTuneId}"), _configuration, cancellationToken);
        public IAsyncEnumerable<FineTuneEventsResult> ListEventsAsStreamAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => Client.StreamAsync<FineTuneEventsResult>(_configuration.GetUri(OpenAiType.FineTune, string.Empty, _forced, string.Empty), null, HttpMethod.Get, _configuration, cancellationToken);
    }
}
