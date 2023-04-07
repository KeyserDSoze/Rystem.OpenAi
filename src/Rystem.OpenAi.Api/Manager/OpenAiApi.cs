using System.Collections.Generic;
using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Completion;
using Rystem.OpenAi.Edit;
using Rystem.OpenAi.Embedding;
using Rystem.OpenAi.Files;
using Rystem.OpenAi.FineTune;
using Rystem.OpenAi.Image;
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
        private readonly List<OpenAiBase> _openAiBases = new List<OpenAiBase>();

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
            if (Completion is OpenAiBase aiBasesForCompletion)
                _openAiBases.Add(aiBasesForCompletion);
            Embedding = embeddingApi;
            if (Embedding is OpenAiBase aiBasesForEmbedding)
                _openAiBases.Add(aiBasesForEmbedding);
            Model = modelApi;
            if (Model is OpenAiBase aiBasesForModel)
                _openAiBases.Add(aiBasesForModel);
            File = fileApi;
            if (File is OpenAiBase aiBasesForFile)
                _openAiBases.Add(aiBasesForFile);
            Image = imageApi;
            if (Image is OpenAiBase aiBasesForImage)
                _openAiBases.Add(aiBasesForImage);
            Moderation = moderationApi;
            if (Moderation is OpenAiBase aiBasesForModeration)
                _openAiBases.Add(aiBasesForModeration);
            Audio = audioApi;
            if (Audio is OpenAiBase aiBasesForAudio)
                _openAiBases.Add(aiBasesForAudio);
            FineTune = fineTuneApi;
            if (FineTune is OpenAiBase aiBasesForFineTune)
                _openAiBases.Add(aiBasesForFineTune);
            Chat = chatApi;
            if (Chat is OpenAiBase aiBasesForChat)
                _openAiBases.Add(aiBasesForChat);
            Edit = editApi;
            if (Edit is OpenAiBase aiBasesForEdit)
                _openAiBases.Add(aiBasesForEdit);
        }
        public void SetName(string? name)
        {
            if (name == null)
                name = string.Empty;
            foreach (var bases in _openAiBases)
                bases.SetName(name);
        }
    }
}
