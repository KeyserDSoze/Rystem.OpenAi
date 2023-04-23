using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.FineTune
{
    internal sealed class OpenAiFineTune : OpenAiBase, IOpenAiFineTune, IOpenAiFineTuneApi
    {
        private readonly bool _forced;
        public OpenAiFineTune(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations, IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
            _forced = false;
        }
        public FineTuneRequestBuilder Create(string trainingFileId)
            => new FineTuneRequestBuilder(Client, Configuration, trainingFileId, Utility);
        public ValueTask<FineTuneResults> ListAsync(CancellationToken cancellationToken = default)
            => Client.GetAsync<FineTuneResults>(Configuration.GetUri(OpenAiType.FineTune, string.Empty, _forced, string.Empty), Configuration, cancellationToken);
        public ValueTask<FineTuneResult> RetrieveAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => Client.GetAsync<FineTuneResult>(Configuration.GetUri(OpenAiType.FineTune, fineTuneId, _forced, $"/{fineTuneId}"), Configuration, cancellationToken);
        public ValueTask<FineTuneResult> CancelAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => Client.PostAsync<FineTuneResult>(Configuration.GetUri(OpenAiType.FineTune, fineTuneId, _forced, $"/{fineTuneId}/cancel"), null, Configuration, cancellationToken);
        public ValueTask<FineTuneEventsResult> ListEventsAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => Client.GetAsync<FineTuneEventsResult>(Configuration.GetUri(OpenAiType.FineTune, fineTuneId, _forced, $"/{fineTuneId}/events"), Configuration, cancellationToken);
        public ValueTask<FineTuneDeleteResult> DeleteAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => Client.DeleteAsync<FineTuneDeleteResult>(Configuration.GetUri(OpenAiType.FineTune, fineTuneId, _forced, $"/{fineTuneId}"), Configuration, cancellationToken);
        public IAsyncEnumerable<FineTuneEventsResult> ListEventsAsStreamAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => Client.StreamAsync<FineTuneEventsResult>(Configuration.GetUri(OpenAiType.FineTune, string.Empty, _forced, string.Empty), null, HttpMethod.Get, Configuration, cancellationToken);
    }
}
