using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiModel : OpenAiBase, IOpenAiModel, IOpenAiModelApi
    {
        private readonly bool _forced;
        public OpenAiModel(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations, IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
            _forced = false;
        }
        public ValueTask<Model> RetrieveAsync(string id, CancellationToken cancellationToken = default)
            => Client.GetAsync<Model>(Configuration.GetUri(OpenAiType.Model, string.Empty, _forced, $"/{id}"), Configuration, cancellationToken);
        public async Task<List<Model>> ListAsync(CancellationToken cancellationToken = default)
        {
            var response = await Client.GetAsync<JsonHelperRoot>(Configuration.GetUri(OpenAiType.Model, string.Empty, _forced, string.Empty), Configuration, cancellationToken);
            return response.Data!;
        }
        private sealed class JsonHelperRoot : ApiBaseResponse
        {
            [JsonPropertyName("data")]
            public List<Model>? Data { get; set; }
        }
    }
}
