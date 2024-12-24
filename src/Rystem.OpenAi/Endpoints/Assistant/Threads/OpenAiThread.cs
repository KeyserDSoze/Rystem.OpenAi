using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiThread : OpenAiBuilder<IOpenAiThread, ThreadRequest, ChatModelName>, IOpenAiThread
    {
        public OpenAiThread(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory)
            : base(factory, configurationFactory, OpenAiType.Assistant)
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
        private ThreadMessage GetLastMessage(ChatRole role)
        {
            Request.Messages ??= [];
            var lastMessage = Request.Messages.LastOrDefault();
            if (lastMessage?.Role != role.AsString())
            {
                lastMessage = new ThreadMessage
                {
                    Role = role.AsString(),
                };
                Request.Messages.Add(lastMessage);
            }
            return lastMessage;
        }
        private List<ChatMessageContent> GetLastContent(ChatRole role)
        {
            var lastMessage = GetLastMessage(role);
            lastMessage.Content ??= new List<ChatMessageContent>();
            return lastMessage.Content.CastT1;
        }
        public IOpenAiThread AddText(ChatRole role, string text)
        {
            var contents = GetLastContent(role);
            contents.Add(new ChatMessageContent { Text = text, Type = ChatConstants.ContentType.Text });
            return this;
        }
        public ChatMessageContentBuilder<IOpenAiThread> AddContent(ChatRole role = ChatRole.User)
        {
            var content = new List<ChatMessageContent>();
            Request.Messages ??= [];
            Request.Messages.Add(new ThreadMessage { Content = content, Role = role.AsString() });
            return new ChatMessageContentBuilder<IOpenAiThread>(this, content);
        }
        public ChatMessageContentBuilder<IOpenAiThread> AddUserContent()
          => AddContent(ChatRole.User);
        public ChatMessageContentBuilder<IOpenAiThread> AddAssistantContent()
          => AddContent(ChatRole.Assistant);
        public IOpenAiThread AddAttachment(ChatRole role, string? fileId, bool withCodeInterpreter = false, bool withFileSearch = false)
        {
            var message = GetLastMessage(role);
            message.Attachments ??= [];
            var attachment = new ThreadAttachment
            {
                FileId = fileId,
            };
            if (withCodeInterpreter || withFileSearch)
            {
                attachment.Tools = [];
                if (withCodeInterpreter)
                    attachment.Tools.Add(ThreadAttachmentTool.CodeInterpreter);
                if (withFileSearch)
                    attachment.Tools.Add(ThreadAttachmentTool.FileSearch);
            }
            message.Attachments.Add(attachment);
            return this;
        }
        public IOpenAiThread AddMetadata(ChatRole role, string key, string value)
        {
            var message = GetLastMessage(role);
            message.Metadata ??= [];
            message.Metadata.TryAdd(key, value);
            return this;
        }
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
            Request.Metadata?.Clear();
            return this;
        }
        public IOpenAiThread RemoveMetadata(string key)
        {
            if (Request.Metadata?.ContainsKey(key) == true)
                Request.Metadata.Remove(key);
            return this;
        }
        public IOpenAiToolResourcesAssistant<IOpenAiThread> WithToolResources()
        {
            Request.ToolResources ??= new();
            return new OpenAiToolResourcesAssistant<IOpenAiThread>(this, Request.ToolResources);
        }
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
