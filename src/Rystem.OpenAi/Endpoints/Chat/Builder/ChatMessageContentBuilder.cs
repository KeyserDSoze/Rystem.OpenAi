using System.Collections.Generic;
using System.IO;
using System.Text;
using Rystem.OpenAi.Audio;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatMessageContentBuilder<T>
    {
        private readonly T _builder;
        private readonly List<ChatMessageContent> _content;
        internal ChatMessageContentBuilder(T builder, List<ChatMessageContent> content)
        {
            _builder = builder;
            _content = content;
        }
        public ChatMessageContentBuilder<T> AddText(string text)
        {
            _content.Add(new ChatMessageContent { Text = text, Type = ChatConstants.ContentType.Text });
            return this;
        }
        public ChatMessageContentBuilder<T> AddImage(string uri, ResolutionForVision resolutionForVision = ResolutionForVision.Low)
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
        public ChatMessageContentBuilder<T> AddFileImage(string fileId, ResolutionForVision resolutionForVision = ResolutionForVision.Low)
        {
            _content.Add(new ChatMessageContent
            {
                FileImage = new ChatMessageImageFile
                {
                    FileId = fileId,
                    Detail = resolutionForVision switch
                    {
                        ResolutionForVision.Auto => ChatConstants.ResolutionVision.Auto,
                        ResolutionForVision.High => ChatConstants.ResolutionVision.High,
                        _ => ChatConstants.ResolutionVision.Low,
                    }
                },
                Type = ChatConstants.ContentType.ImageFile
            });
            return this;
        }
        public ChatMessageContentBuilder<T> AddAudio(Stream stream, AudioFormat audioFormat = AudioFormat.Mp3)
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
        public T And => _builder;
    }
}
