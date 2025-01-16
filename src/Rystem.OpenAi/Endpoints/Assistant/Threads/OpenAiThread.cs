using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiThread : OpenAiBuilder<IOpenAiThread, ThreadRequest, ChatModelName>, IOpenAiThread
    {
        private string? _threadId;
        public OpenAiThread(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLoggerFactory loggerFactory)
            : base(factory, configurationFactory, loggerFactory, OpenAiType.Thread)
        {
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.Thread != null)
            {
                configuration.Settings.DefaultRequestConfiguration.Thread.Invoke(this);
            }
        }
        public IMessageThreadBuilder<IOpenAiThread> WithMessage()
            => new MessageThreadBuilder<IOpenAiThread>(this, Request);
        public IOpenAiThread AddMetadata(string key, string value)
        {
            Request.Metadata ??= [];
            Request.Metadata.TryAdd(key, value);
            return this;
        }

        public IOpenAiThread AddMetadata(Dictionary<string, string> metadata)
        {
            Request.Metadata = metadata;
            return this;
        }
        public IOpenAiThread ClearMetadata()
        {
            Request.Metadata = null;
            return this;
        }
        public IOpenAiThread RemoveMetadata(string key)
        {
            Request.Metadata?.Remove(key);
            return this;
        }
        public IOpenAiToolResourcesAssistant<IOpenAiThread> WithToolResources()
        {
            Request.ToolResources ??= new();
            return new OpenAiToolResourcesAssistant<IOpenAiThread>(this, Request.ToolResources);
        }
        public IOpenAiThread WithId(string id)
        {
            _threadId = id;
            return this;
        }
        public async ValueTask<ThreadResponse> CreateAsync(CancellationToken cancellationToken = default)
        {
            var threadResponse = await DefaultServices.HttpClientWrapper.
                PostAsync<ThreadResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, string.Empty, null),
                        Request,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
            _threadId = threadResponse.Id;
            return threadResponse;
        }

        public ValueTask<DeleteResponse> DeleteAsync(CancellationToken cancellationToken = default)
        {
            if (_threadId == null)
                throw new Exception("Thread Id is required to delete a thread. Please use WithId method.");
            return DefaultServices.HttpClientWrapper.
                DeleteAsync<DeleteResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }
        public ValueTask<ThreadResponse> RetrieveAsync(CancellationToken cancellationToken = default)
        {
            if (_threadId == null)
                throw new Exception("Thread Id is required to delete a thread. Please use WithId method.");
            return DefaultServices.HttpClientWrapper.
                GetAsync<ThreadResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }

        public ValueTask<ThreadResponse> UpdateAsync(CancellationToken cancellationToken = default)
        {
            if (_threadId == null)
                throw new Exception("Thread Id is required to delete a thread. Please use WithId method.");
            return DefaultServices.HttpClientWrapper.
                PostAsync<ThreadResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}", null),
                        Request,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }
        public async IAsyncEnumerable<ThreadMessageResponse> AddMessagesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (_threadId == null)
                throw new Exception("Thread Id is required to delete a thread. Please use WithId method.");
            if (Request.Messages != null)
                foreach (var message in Request.Messages.Where(x => !x.AlreadyAdded))
                {
                    var response = await DefaultServices.HttpClientWrapper.
                                            PostAsync<ThreadMessageResponse>(
                                                DefaultServices.Configuration.GetUri(
                                                    OpenAiType.Thread, _version, null, $"/{_threadId}/messages", null),
                                                    message,
                                                    BetaRequest.OpenAiBetaHeaders,
                                                    DefaultServices.Configuration,
                                                    LoggerFactory.Create(),
                                                    cancellationToken);
                    yield return response;
                }
        }

        public ValueTask<DeleteResponse> DeleteMessageAsync(string id, CancellationToken cancellationToken = default)
        {
            if (_threadId == null)
                throw new Exception("Thread Id is required to delete a thread. Please use WithId method.");
            return DefaultServices.HttpClientWrapper.
                DeleteAsync<DeleteResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/messages/{id}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }
        public ValueTask<ThreadMessageResponse> RetrieveMessageAsync(string id, CancellationToken cancellationToken = default)
        {
            if (_threadId == null)
                throw new Exception("Thread Id is required to delete a thread. Please use WithId method.");
            return DefaultServices.HttpClientWrapper.
                GetAsync<ThreadMessageResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/messages/{id}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }
        public ValueTask<ResponseAsArray<ThreadMessageResponse>> ListMessagesAsync(int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default)
        {
            if (_threadId == null)
                throw new Exception("Thread Id is required to delete a thread. Please use WithId method.");
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
                GetAsync<ResponseAsArray<ThreadMessageResponse>>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/messages", querystring),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }
        public ValueTask<ThreadMessageResponse> UpdateMessageAsync(string id, CancellationToken cancellationToken = default)
        {
            if (_threadId == null)
                throw new Exception("Thread Id is required to delete a thread. Please use WithId method.");
            var message = (Request.Messages?.LastOrDefault(x => !x.AlreadyAdded)) ?? throw new Exception("Cannot update a message without a message. Please use WithMessage method.");
            return DefaultServices.HttpClientWrapper.
                PostAsync<ThreadMessageResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/messages/{id}", null),
                        Request.Messages?.LastOrDefault(),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }
    }
}
