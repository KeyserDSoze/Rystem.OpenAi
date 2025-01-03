using System.Collections.Generic;
using System.Linq;
using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    public sealed class ThreadHelper<T>
    {
        private readonly T _entity;
        internal ThreadRequest Request { get; }
        internal ThreadHelper(T entity, ThreadRequest request)
        {
            _entity = entity;
            Request = request;
        }
        public T Builder => _entity;
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
        public T AddText(ChatRole role, string text)
        {
            var contents = GetLastContent(role);
            contents.Add(new ChatMessageContent { Text = text, Type = ChatConstants.ContentType.Text });
            return _entity;
        }
        public ChatMessageContentBuilder<T> AddContent(ChatRole role = ChatRole.User)
        {
            var content = new List<ChatMessageContent>();
            Request.Messages ??= [];
            Request.Messages.Add(new ThreadMessage { Content = content, Role = role.AsString() });
            return new ChatMessageContentBuilder<T>(_entity, content);
        }
        public ChatMessageContentBuilder<T> AddUserContent()
          => AddContent(ChatRole.User);
        public ChatMessageContentBuilder<T> AddAssistantContent()
          => AddContent(ChatRole.Assistant);
        public T AddAttachment(ChatRole role, string? fileId, bool withCodeInterpreter = false, bool withFileSearch = false)
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
            return _entity;
        }
        public T AddMetadata(ChatRole role, string key, string value)
        {
            var message = GetLastMessage(role);
            message.Metadata ??= [];
            message.Metadata.TryAdd(key, value);
            return _entity;
        }
        public T AddMetadata(string key, string value)
        {
            Request.Metadata ??= [];
            Request.Metadata.TryAdd(key, value);
            return _entity;
        }
        public T AddMetadata(Dictionary<string, string> metadata)
        {
            Request.Metadata = metadata;
            return _entity;
        }
        public T ClearMetadata()
        {
            Request.Metadata?.Clear();
            return _entity;
        }
        public T RemoveMetadata(string key)
        {
            if (Request.Metadata?.ContainsKey(key) == true)
                Request.Metadata.Remove(key);
            return _entity;
        }
        public IOpenAiToolResourcesAssistant<T> WithToolResources()
        {
            Request.ToolResources ??= new();
            return new OpenAiToolResourcesAssistant<T>(_entity, Request.ToolResources);
        }
    }
}
