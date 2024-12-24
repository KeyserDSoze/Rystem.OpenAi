using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiMessage : OpenAiBuilder<IOpenAiMessage, ThreadMessage, ChatModelName>, IOpenAiMessage
    {
        public OpenAiMessage(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory)
            : base(factory, configurationFactory, OpenAiType.Assistant)
        {
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.Message != null)
            {
                configuration.Settings.DefaultRequestConfiguration.Message.Invoke(this);
            }
        }
        public IOpenAiMessage AddText(ChatRole role, string text)
        {
            Request.Content ??= new List<ChatMessageContent>();
            Request.Content.AsT1!.Add(new ChatMessageContent { Text = text, Type = ChatConstants.ContentType.Text });
            return this;
        }
        public ChatMessageContentBuilder<IOpenAiMessage> AddContent(ChatRole role = ChatRole.User)
        {
            Request.Content ??= new List<ChatMessageContent>();
            return new ChatMessageContentBuilder<IOpenAiMessage>(this, Request.Content.AsT1!);
        }
        public ChatMessageContentBuilder<IOpenAiMessage> AddUserContent()
          => AddContent(ChatRole.User);
        public ChatMessageContentBuilder<IOpenAiMessage> AddAssistantContent()
          => AddContent(ChatRole.Assistant);
        public IOpenAiMessage AddAttachment(ChatRole role, string? fileId, bool withCodeInterpreter = false, bool withFileSearch = false)
        {
            Request.Attachments ??= [];
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
            Request.Attachments.Add(attachment);
            return this;
        }
        public IOpenAiMessage AddMetadata(string key, string value)
        {
            Request.Metadata ??= [];
            Request.Metadata.TryAdd(key, value);
            return this;
        }
        public IOpenAiMessage AddMetadata(Dictionary<string, string> metadata)
        {
            Request.Metadata = metadata;
            return this;
        }
        public IOpenAiMessage ClearMetadata()
        {
            Request.Metadata?.Clear();
            return this;
        }
        public IOpenAiMessage RemoveMetadata(string key)
        {
            if (Request.Metadata?.ContainsKey(key) == true)
                Request.Metadata.Remove(key);
            return this;
        }
        private static readonly Dictionary<string, string> s_betaHeaders = new()
        {
            { "OpenAI-Beta", "assistants=v2" }
        };
        public ValueTask<ThreadMessageResponse> CreateAsync(string threadId, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                PostAsync<ThreadMessageResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, string.Empty, Forced, $"/{threadId}/messages", null),
                        Request,
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }

        public ValueTask<DeleteResponse> DeleteAsync(string threadId, string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                DeleteAsync<DeleteResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, string.Empty, Forced, $"/{threadId}/messages/{id}", null),
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
        public ValueTask<ThreadMessageResponse> RetrieveAsync(string threadId, string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                GetAsync<ThreadMessageResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, string.Empty, Forced, $"/{threadId}/messages/{id}", null),
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
        public ValueTask<ResponseAsArray<ThreadMessageResponse>> ListAsync(string threadId, int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default)
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
                GetAsync<ResponseAsArray<ThreadMessageResponse>>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, string.Empty, Forced, $"/{threadId}/messages", querystring),
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }

        public ValueTask<ThreadMessageResponse> UpdateAsync(string threadId, string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                PostAsync<ThreadMessageResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, string.Empty, Forced, $"/{threadId}/messages/{id}", null),
                        Request,
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
    }
}
