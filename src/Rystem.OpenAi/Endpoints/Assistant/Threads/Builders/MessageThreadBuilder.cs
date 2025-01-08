using System;
using System.Collections.Generic;
using System.Linq;
using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    public sealed class MessageThreadBuilder<T> : IMessageThreadBuilder<T>
    {
        private readonly T _entity;
        internal ThreadRequest Request { get; }
        internal MessageThreadBuilder(T entity, ThreadRequest request)
        {
            _entity = entity;
            Request = request;
        }
        public T Thread => _entity;
        private ThreadMessage? GetLastMessage(ChatRole? role = null)
        {
            Request.Messages ??= [];
            var lastMessage = Request.Messages.LastOrDefault();
            if (role != null && lastMessage?.Role != role.Value.AsString())
            {
                lastMessage = new ThreadMessage
                {
                    Role = role.Value.AsString(),
                    AlreadyAdded = false
                };
                Request.Messages.Add(lastMessage);
            }
            return lastMessage;
        }
        private List<ChatMessageContent>? GetLastContent(ChatRole role)
        {
            var lastMessage = GetLastMessage(role);
            if (lastMessage != null)
            {
                lastMessage.Content ??= new List<ChatMessageContent>();
                return lastMessage.Content.CastT1;
            }
            return null;
        }
        public IMessageThreadBuilder<T> AddEmpty(ChatRole role)
        {
            Request.Messages ??= [];
            Request.Messages.Add(new ThreadMessage { Role = role.AsString(), AlreadyAdded = false });
            return this;
        }
        public T AddText(ChatRole role, string text)
        {
            var contents = GetLastContent(role)!;
            contents.Add(new ChatMessageContent { Text = text, Type = ChatConstants.ContentType.Text });
            return _entity;
        }
        public T Add(ChatMessage message)
        {
            Request.Messages ??= [];
            Request.Messages.Add(new ThreadMessage
            {
                Content = message.Content,
                AlreadyAdded = false,
                Role = message.Role.AsString(),
            });
            return _entity;
        }
        public T Add(ThreadMessage message)
        {
            Request.Messages ??= [];
            Request.Messages.Add(message);
            return _entity;
        }
        public ChatMessageContentBuilder<IMessageThreadBuilder<T>> AddContent(ChatRole role = ChatRole.User)
        {
            var content = new List<ChatMessageContent>();
            Request.Messages ??= [];
            Request.Messages.Add(new ThreadMessage { Content = content, Role = role.AsString(), AlreadyAdded = false });
            return new ChatMessageContentBuilder<IMessageThreadBuilder<T>>(this, content);
        }
        public ChatMessageContentBuilder<IMessageThreadBuilder<T>> AddUserContent()
          => AddContent(ChatRole.User);
        public ChatMessageContentBuilder<IMessageThreadBuilder<T>> AddAssistantContent()
          => AddContent(ChatRole.Assistant);
        public IMessageThreadBuilder<T> AddAttachment(string? fileId, bool withCodeInterpreter = false, bool withFileSearch = false)
        {
            var message = GetLastMessage(null) ?? throw new InvalidOperationException("Cannot add attachment without a message.");
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
        public IMessageThreadBuilder<T> AddMetadata(string key, string value)
        {
            var message = GetLastMessage(null) ?? throw new InvalidOperationException("Cannot add metadata without a message."); ;
            message.Metadata ??= [];
            message.Metadata.TryAdd(key, value);
            return this;
        }
        public IMessageThreadBuilder<T> AddMetadata(Dictionary<string, string> metadata)
        {
            var message = GetLastMessage(null) ?? throw new InvalidOperationException("Cannot add metadata without a message."); ;
            message.Metadata = metadata;
            return this;
        }
        public IMessageThreadBuilder<T> ClearMetadata()
        {
            var message = GetLastMessage(null) ?? throw new InvalidOperationException("Cannot clear metadata without a message."); ;
            message.Metadata = null;
            return this;
        }
        public IMessageThreadBuilder<T> RemoveMetadata(string key)
        {
            var message = GetLastMessage(null) ?? throw new InvalidOperationException("Cannot remove metadata without a message.");
            message.Metadata?.Remove(key);
            return this;
        }
    }
}
