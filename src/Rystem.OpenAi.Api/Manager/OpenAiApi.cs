using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Completion;
using Rystem.OpenAi.Edit;
using Rystem.OpenAi.Embedding;
using Rystem.OpenAi.File;
using Rystem.OpenAi.FineTune;
using Rystem.OpenAi.Image;
using Rystem.OpenAi.Models;
using Rystem.OpenAi.Moderation;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiApi : IOpenAiApi
    {
        public IOpenAiModelApi Model { get; }
        public IOpenAiCompletionApi Completion { get; }
        public IOpenAiImageApi Image { get; }
        public IOpenAiEmbeddingApi Embedding { get; }
        public IOpenAiFileApi File { get; }
        public IOpenAiModerationApi Moderation { get; }
        public IOpenAiAudioApi Audio { get; }
        public IOpenAiFineTuneApi FineTune { get; }
        public IOpenAiChatApi Chat { get; }
        public IOpenAiEditApi Edit { get; }

        public OpenAiApi(IOpenAiCompletionApi completionApi,
            IOpenAiEmbeddingApi embeddingApi,
            IOpenAiModelApi modelApi,
            IOpenAiFileApi fileApi,
            IOpenAiImageApi imageApi,
            IOpenAiModerationApi moderationApi,
            IOpenAiAudioApi audioApi,
            IOpenAiFineTuneApi fineTuneApi,
            IOpenAiChatApi chatApi,
            IOpenAiEditApi editApi)
        {
            Completion = completionApi;
            Embedding = embeddingApi;
            Model = modelApi;
            File = fileApi;
            Image = imageApi;
            Moderation = moderationApi;
            Audio = audioApi;
            FineTune = fineTuneApi;
            Chat = chatApi;
            Edit = editApi;
        }
    }
}
