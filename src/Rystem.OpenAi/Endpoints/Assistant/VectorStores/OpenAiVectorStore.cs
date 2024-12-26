using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiVectorStore : OpenAiBuilderWithMetadat<IOpenAiVectorStore, VectorStoreRequest, ChatModelName>, IOpenAiVectorStore
    {
        public OpenAiVectorStore(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory)
            : base(factory, configurationFactory, OpenAiType.VectorStore)
        {
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.VectorStore != null)
            {
                configuration.Settings.DefaultRequestConfiguration.VectorStore.Invoke(this);
            }
        }
        public IOpenAiVectorStore WithName(string name)
        {
            Request.Name = name;
            return this;
        }
        public IOpenAiVectorStore AddFile(string fileId)
        {
            Request.FileIds ??= [];
            Request.FileIds.Add(fileId);
            return this;
        }
        public IOpenAiVectorStore AddFiles(IEnumerable<string> fileIds)
        {
            Request.FileIds ??= [];
            Request.FileIds.AddRange(fileIds);
            return this;
        }

        private const string LastActiveAtAnchor = "last_active_at";
        public IOpenAiVectorStore WithExpirationAfter(int days)
        {
            Request.ExpiresAfter = new VectoStoreExpirationPolicy
            {
                Days = days,
                Anchor = LastActiveAtAnchor
            };
            return this;
        }

        private static readonly Dictionary<string, string> s_betaHeaders = new()
        {
            { "OpenAI-Beta", "assistants=v2" }
        };

        public ValueTask<VectorStoreResult> CreateAsync(CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                PostAsync<VectorStoreResult>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, string.Empty, Forced, string.Empty, null),
                        Request,
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
        public ValueTask<DeleteResponse> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                DeleteAsync<DeleteResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, string.Empty, Forced, $"/{id}", null),
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
        public ValueTask<ResponseAsArray<VectorStoreResult>> ListAsync(int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default)
        {
            var querystring = new Dictionary<string, string>
            {
                { "limit", take.ToString() },
                { "order", order == AssistantOrder.Descending ? "desc" : "asc" },
            };
            if (elementId != null && getAfterTheElementId)
                querystring.Add("after", elementId);
            else if (elementId != null && !getAfterTheElementId)
                querystring.Add("before", elementId);
            return DefaultServices.HttpClientWrapper.
                GetAsync<ResponseAsArray<VectorStoreResult>>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, string.Empty, Forced, string.Empty, querystring),
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
        public ValueTask<VectorStoreResult> RetrieveAsync(string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                GetAsync<VectorStoreResult>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, string.Empty, Forced, $"/{id}", null),
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
        public ValueTask<AssistantRequest> UpdateAsync(string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                PostAsync<AssistantRequest>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, string.Empty, Forced, $"/{id}", null),
                        Request,
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
    }
}
