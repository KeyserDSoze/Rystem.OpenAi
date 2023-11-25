using System.Collections.Generic;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageContentBuilder
    {
        private readonly ChatRequestBuilder _requestBuilder;
        private readonly List<ChatMessageContent> _content;
        public ChatMessageContentBuilder(ChatRequestBuilder requestBuilder, ChatRole role)
        {
            _requestBuilder = requestBuilder;
            _content = new List<ChatMessageContent>();
            _requestBuilder.AddMessage(new ChatMessage { Content = _content, Role = role });
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
        public ChatRequestBuilder Builder => _requestBuilder;
    }
}
