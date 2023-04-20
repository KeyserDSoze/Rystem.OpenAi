using System.Collections.Generic;
using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Completion;
using Rystem.OpenAi.Edit;
using Rystem.OpenAi.Embedding;
using Rystem.OpenAi.Files;
using Rystem.OpenAi.FineTune;
using Rystem.OpenAi.Image;
using Rystem.OpenAi.Management;
using Rystem.OpenAi.Moderation;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiApi : IOpenAi, IOpenAiApi
    {
        public IOpenAiModel Model { get; }
        public IOpenAiCompletion Completion { get; }
        public IOpenAiImage Image { get; }
        public IOpenAiEmbedding Embedding { get; }
        public IOpenAiFile File { get; }
        public IOpenAiModeration Moderation { get; }
        public IOpenAiAudio Audio { get; }
        public IOpenAiFineTune FineTune { get; }
        public IOpenAiChat Chat { get; }
        public IOpenAiEdit Edit { get; }
        public IOpenAiManagement Management { get; }
        private readonly List<OpenAiBase> _openAiBases = new List<OpenAiBase>();

        public OpenAiApi(IOpenAiCompletion completionApi,
            IOpenAiEmbedding embeddingApi,
            IOpenAiModel modelApi,
            IOpenAiFile fileApi,
            IOpenAiImage imageApi,
            IOpenAiModeration moderationApi,
            IOpenAiAudio audioApi,
            IOpenAiFineTune fineTuneApi,
            IOpenAiChat chatApi,
            IOpenAiEdit editApi,
            IOpenAiManagement managementApi)
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
            Management = managementApi;
            if (Management is OpenAiBase aiBasesForManagement)
                _openAiBases.Add(aiBasesForManagement);
        }
        public void SetName(string? name)
        {
            name ??= string.Empty;
            foreach (var bases in _openAiBases)
                bases.SetName(name);
        }
    }
}
