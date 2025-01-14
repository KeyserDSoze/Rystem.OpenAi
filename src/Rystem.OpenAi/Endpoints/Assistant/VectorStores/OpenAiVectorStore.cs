using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiVectorStore : OpenAiBuilderWithMetadata<IOpenAiVectorStore, VectorStoreRequest, ChatModelName>, IOpenAiVectorStore
    {
        private string? _vectorStoreId;
        public OpenAiVectorStore(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLoggerFactory loggerFactory)
            : base(factory, configurationFactory, loggerFactory, OpenAiType.VectorStore)
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
        public IOpenAiVectorStoreFile ManageStore()
        {
            if (_vectorStoreId == null)
                throw new System.Exception("Vector Store Id is required. Use WithId method.");
            return new OpenAiVectorStoreFile(_vectorStoreId, DefaultServices, LoggerFactory, _version);
        }

        public IOpenAiVectorStore WithId(string id)
        {
            _vectorStoreId = id;
            return this;
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
        public async ValueTask<VectorStoreResult> CreateAsync(CancellationToken cancellationToken = default)
        {
            var response = await DefaultServices.HttpClientWrapper.
                PostAsync<VectorStoreResult>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, _version, null, string.Empty, null),
                        Request,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
            _vectorStoreId = response.Id;
            return response;
        }
        public ValueTask<DeleteResponse> DeleteAsync(CancellationToken cancellationToken = default)
        {
            if (_vectorStoreId == null)
                throw new System.Exception("Vector Store Id is required. Use WithId method.");
            return DefaultServices.HttpClientWrapper.
                DeleteAsync<DeleteResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, _version, null, $"/{_vectorStoreId}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
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
                        OpenAiType.VectorStore, _version, null, string.Empty, querystring),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }
        public ValueTask<VectorStoreResult> RetrieveAsync(CancellationToken cancellationToken = default)
        {
            if (_vectorStoreId == null)
                throw new System.Exception("Vector Store Id is required. Use WithId method.");
            return DefaultServices.HttpClientWrapper.
                GetAsync<VectorStoreResult>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, _version, null, $"/{_vectorStoreId}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }
        public ValueTask<VectorStoreResult> UpdateAsync(CancellationToken cancellationToken = default)
        {
            if (_vectorStoreId == null)
                throw new System.Exception("Vector Store Id is required. Use WithId method.");
            return DefaultServices.HttpClientWrapper.
                PostAsync<VectorStoreResult>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, _version, null, $"/{_vectorStoreId}", null),
                        Request,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }
    }
}
