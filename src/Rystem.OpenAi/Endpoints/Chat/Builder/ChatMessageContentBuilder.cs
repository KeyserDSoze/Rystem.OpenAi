using System.Collections.Generic;
using System.IO;
using System.Text;
using Rystem.OpenAi.Audio;

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
            _openAiChat.AddMessage(new ChatMessageRequest { Content = _content, Role = role });
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
        public ChatMessageContentBuilder AddAudio(Stream stream, AudioFormat audioFormat = AudioFormat.Mp3)
        {
            _content.Add(new ChatMessageContent
            {
                AudioInput = new ChatMessageAudioFile
                {
                    Data = stream.ToBase64(),
                    Format = audioFormat.AsString()
                },
                Type = ChatConstants.ContentType.AudioInput
            });
            return this;
        }
        public IOpenAiChat Builder => _openAiChat;
    }
}
