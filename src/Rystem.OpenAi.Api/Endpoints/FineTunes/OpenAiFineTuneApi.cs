using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.FineTune
{
    internal sealed class OpenAiFineTuneApi : IOpenAiFineTuneApi
    {
        private readonly HttpClient _client;
        private readonly OpenAiConfiguration _configuration;
        private readonly bool _forced;
        public OpenAiFineTuneApi(IHttpClientFactory httpClientFactory, OpenAiConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            _configuration = configuration;
        }
        public FineTuneRequestBuilder Create(string trainingFileId)
            => new FineTuneRequestBuilder(_client, _configuration, trainingFileId);
        public ValueTask<FineTuneResults> ListAsync(CancellationToken cancellationToken = default)
            => _client.GetAsync<FineTuneResults>(_configuration.GetUri(OpenAiType.FineTune, string.Empty, _forced), _configuration, cancellationToken);
        public ValueTask<FineTuneResult> RetrieveAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => _client.GetAsync<FineTuneResult>($"{_configuration.GetUri(OpenAiType.FineTune, fineTuneId, _forced)}/{fineTuneId}", _configuration, cancellationToken);
        public ValueTask<FineTuneResult> CancelAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => _client.PostAsync<FineTuneResult>($"{_configuration.GetUri(OpenAiType.FineTune, fineTuneId, _forced)}/{fineTuneId}/cancel", null, _configuration, cancellationToken);
        public ValueTask<FineTuneEventsResult> ListEventsAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => _client.GetAsync<FineTuneEventsResult>($"{_configuration.GetUri(OpenAiType.FineTune, fineTuneId, _forced)}/{fineTuneId}/events", _configuration, cancellationToken);
        public ValueTask<FineTuneDeleteResult> DeleteAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => _client.DeleteAsync<FineTuneDeleteResult>($"{_configuration.GetUri(OpenAiType.Model, fineTuneId, _forced)}/{fineTuneId}", _configuration, cancellationToken);
        public IAsyncEnumerable<FineTuneEventsResult> ListEventsAsStreamAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => _client.StreamAsync<FineTuneEventsResult>(_configuration.GetUri(OpenAiType.FineTune, string.Empty, _forced), null, HttpMethod.Get, _configuration, cancellationToken);
    }
}
