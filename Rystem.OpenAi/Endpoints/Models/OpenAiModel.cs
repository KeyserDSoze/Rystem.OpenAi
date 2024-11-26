using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.FineTune;

namespace Rystem.OpenAi.Models
{
    internal sealed class OpenAiModel : OpenAiBuilder<IOpenAiModel>, IOpenAiModel
    {
        public OpenAiModel(IFactory<DefaultServices> factory) : base(factory)
        {
        }
        public ValueTask<Model> RetrieveAsync(string id, CancellationToken cancellationToken = default)
            => DefaultServices.HttpClient.GetAsync<Model>(DefaultServices.Configuration.GetUri(OpenAiType.Model, string.Empty, Forced, $"/{id}"), DefaultServices.Configuration, cancellationToken);
        public async Task<ModelListResult> ListAsync(CancellationToken cancellationToken = default)
        {
            var response = await DefaultServices.HttpClient.GetAsync<ModelListResult>(DefaultServices.Configuration.GetUri(OpenAiType.Model, string.Empty, Forced, string.Empty), DefaultServices.Configuration, cancellationToken);
            return response;
        }
        public ValueTask<FineTuneDeleteResult> DeleteAsync(string fineTuneId, CancellationToken cancellationToken = default)
           => DefaultServices.HttpClient.DeleteAsync<FineTuneDeleteResult>(DefaultServices.Configuration.GetUri(OpenAiType.Model, fineTuneId, Forced, $"/{fineTuneId}"), DefaultServices.Configuration, cancellationToken);
    }
}
