using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiVectorStoreFile : IOpenAiVectorStoreFile
    {
        private readonly string _vectorStoreId;
        private readonly DefaultServices _defaultServices;
        private readonly IOpenAiLogger _logger;
        private readonly VectorStoreFileRequest _request = new();
        private string? _version;
        public OpenAiVectorStoreFile(string vectorStoreId, DefaultServices defaultServices, IOpenAiLogger logger, string? version)
        {
            _vectorStoreId = vectorStoreId;
            _defaultServices = defaultServices;
            _logger = logger;
            _version = version;
        }
        public IOpenAiVectorStoreFile WithVersion(string version)
        {
            _version = version;
            return this;
        }
        public IOpenAiVectorStoreFile WithFile(string fileId)
        {
            _request.Ids = null;
            _request.Id = fileId;
            return this;
        }
        public IOpenAiVectorStoreFile WithFiles(IEnumerable<string> fileIds)
        {
            _request.Id = null;
            _request.Ids ??= [];
            _request.Ids.AddRange(fileIds);
            return this;
        }
        public ValueTask<VectorStoreFile> CreateAsync(CancellationToken cancellationToken = default)
        {
            return _defaultServices.HttpClientWrapper.
                PostAsync<VectorStoreFile>(
                    _defaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, _version, null, $"/{_vectorStoreId}/files", null),
                        _request,
                        BetaRequest.OpenAiBetaHeaders,
                        _defaultServices.Configuration,
                        _logger,
                        cancellationToken);
        }
        public ValueTask<DeleteResponse> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return _defaultServices.HttpClientWrapper.
                DeleteAsync<DeleteResponse>(
                    _defaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, _version, null, $"/{_vectorStoreId}/files/{id}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        _defaultServices.Configuration,
                        _logger,
                        cancellationToken);
        }
        public ValueTask<ResponseAsArray<VectorStoreFile>> ListAsync(int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default)
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
            return _defaultServices.HttpClientWrapper.
                GetAsync<ResponseAsArray<VectorStoreFile>>(
                    _defaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, _version, null, $"/{_vectorStoreId}/files", querystring),
                        BetaRequest.OpenAiBetaHeaders,
                        _defaultServices.Configuration,
                        _logger,
                        cancellationToken);
        }
        public ValueTask<VectorStoreFile> RetrieveAsync(string id, CancellationToken cancellationToken = default)
        {
            return _defaultServices.HttpClientWrapper.
                GetAsync<VectorStoreFile>(
                    _defaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, _version, null, $"/{_vectorStoreId}/files/{id}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        _defaultServices.Configuration,
                        _logger,
                        cancellationToken);
        }
        public ValueTask<VectorStoreFileBatch> CreateBatchAsync(CancellationToken cancellationToken = default)
        {
            return _defaultServices.HttpClientWrapper.
                PostAsync<VectorStoreFileBatch>(
                    _defaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, _version, null, $"/{_vectorStoreId}/file_batches", null),
                        _request,
                        BetaRequest.OpenAiBetaHeaders,
                        _defaultServices.Configuration,
                        _logger,
                        cancellationToken);
        }
        public ValueTask<VectorStoreFileBatch> CancelBatchAsync(string id, CancellationToken cancellationToken = default)
        {
            return _defaultServices.HttpClientWrapper.
                 PostAsync<VectorStoreFileBatch>(
                     _defaultServices.Configuration.GetUri(
                         OpenAiType.VectorStore, _version, null, $"/{_vectorStoreId}/file_batches/{id}/cancel", null),
                         null,
                         BetaRequest.OpenAiBetaHeaders,
                         _defaultServices.Configuration,
                         _logger,
                         cancellationToken);
        }
        public ValueTask<ResponseAsArray<VectorStoreFile>> ListFilesInBatchAsync(string batchId, int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default)
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
            return _defaultServices.HttpClientWrapper.
                GetAsync<ResponseAsArray<VectorStoreFile>>(
                    _defaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, _version, null, $"/{_vectorStoreId}/file_batches/{batchId}/files", querystring),
                        BetaRequest.OpenAiBetaHeaders,
                        _defaultServices.Configuration,
                        _logger,
                        cancellationToken);
        }
        public ValueTask<VectorStoreFileBatch> RetrieveBatchAsync(string id, CancellationToken cancellationToken = default)
        {
            return _defaultServices.HttpClientWrapper.
                GetAsync<VectorStoreFileBatch>(
                    _defaultServices.Configuration.GetUri(
                        OpenAiType.VectorStore, _version, null, $"/{_vectorStoreId}/file_batches/{id}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        _defaultServices.Configuration,
                        _logger,
                        cancellationToken);
        }
    }
}
