using System.Collections.Generic;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageContentBuilder
    {
        private readonly IOpenAiChat _openAiChat;
        private readonly List<ChatMessageContent> _content;
        internal ChatMessageContentBuilder(IOpenAiChat openAiChat, ChatRole role)
        {
            _openAiChat = openAiChat;
            _content = [];
            _openAiChat.AddMessage(new ChatMessage { Content = _content, Role = role });
        }
        public ChatMessageContentBuilder AddText(string text)
        {
            _content.Add(new ChatMessageContent { Text = text, Type = ChatConstants.ContentType.Text });
            return this;
        }
        public ChatMessageContentBuilder AddImage(string uri, ResolutionForVision resolutionForVision = ResolutionForVision.Low)
        {
            _content.Add(new ChatMessageContent
            {
                Image = new ChatMessageImageContent
                {
                    Url = uri,
                    Detail = resolutionForVision switch
                    {
                        ResolutionForVision.Auto => ChatConstants.ResolutionVision.Auto,
                        ResolutionForVision.High => ChatConstants.ResolutionVision.High,
                        _ => ChatConstants.ResolutionVision.Low,
                    }
                },
                Type = ChatConstants.ContentType.Image
            });
            return this;
        }
        public IOpenAiChat Builder => _openAiChat;
    }
}
