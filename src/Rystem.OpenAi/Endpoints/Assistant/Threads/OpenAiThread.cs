using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiThread : OpenAiBuilder<IOpenAiThread, ThreadRequest, ChatModelName>, IOpenAiThread
    {
        private readonly ThreadHelper<IOpenAiThread> _threadHelper;
        public OpenAiThread(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory)
            : base(factory, configurationFactory, OpenAiType.Thread)
        {
            _threadHelper = new ThreadHelper<IOpenAiThread>(this, Request);
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.Thread != null)
            {
                configuration.Settings.DefaultRequestConfiguration.Thread.Invoke(this);
            }
        }
        public IOpenAiThread AddText(ChatRole role, string text)
            => _threadHelper.AddText(role, text);
        public ChatMessageContentBuilder<IOpenAiThread> AddContent(ChatRole role = ChatRole.User)
            => _threadHelper.AddContent(role);
        public ChatMessageContentBuilder<IOpenAiThread> AddUserContent()
          => _threadHelper.AddUserContent();
        public ChatMessageContentBuilder<IOpenAiThread> AddAssistantContent()
          => _threadHelper.AddAssistantContent();
        public IOpenAiThread AddAttachment(ChatRole role, string? fileId, bool withCodeInterpreter = false, bool withFileSearch = false)
            => _threadHelper.AddAttachment(role, fileId, withCodeInterpreter, withFileSearch);
        public IOpenAiThread AddMetadata(ChatRole role, string key, string value)
            => _threadHelper.AddMetadata(role, key, value);
        public IOpenAiThread AddMetadata(string key, string value)
            => _threadHelper.AddMetadata(key, value);
        public IOpenAiThread AddMetadata(Dictionary<string, string> metadata)
            => _threadHelper.AddMetadata(metadata);
        public IOpenAiThread ClearMetadata()
            => _threadHelper.ClearMetadata();
        public IOpenAiThread RemoveMetadata(string key)
            => _threadHelper.RemoveMetadata(key);
        public IOpenAiToolResourcesAssistant<IOpenAiThread> WithToolResources()
            => _threadHelper.WithToolResources();
        private static readonly Dictionary<string, string> s_betaHeaders = new()
        {
            { "OpenAI-Beta", "assistants=v2" }
        };
        public ValueTask<ThreadResponse> CreateAsync(CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                PostAsync<ThreadResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, string.Empty, Forced, string.Empty, null),
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
                        OpenAiType.Thread, string.Empty, Forced, $"/{id}", null),
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
        public ValueTask<ThreadResponse> RetrieveAsync(string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                GetAsync<ThreadResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, string.Empty, Forced, $"/{id}", null),
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }

        public ValueTask<ThreadResponse> UpdateAsync(string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                PostAsync<ThreadResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, string.Empty, Forced, $"/{id}", null),
                        Request,
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
    }
}
