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
    internal sealed class OpenAiFactory : IOpenAiFactory
    {
        private readonly IOpenAiApi _openAiApi;

        public OpenAiFactory(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        public IOpenAiApi Create(string? name = default)
        {
            name ??= string.Empty;
            if (_openAiApi is OpenAiApi internalImplementation)
                internalImplementation.SetName(name);
            return _openAiApi;
        }
        public IOpenAiAudioApi CreateAudio(string? name = null)
            => Create(name).Audio;
        public IOpenAiChatApi CreateChat(string? name = null)
            => Create(name).Chat;
        public IOpenAiCompletionApi CreateCompletion(string? name = null)
            => Create(name).Completion;
        public IOpenAiEditApi CreateEdit(string? name = null)
            => Create(name).Edit;
        public IOpenAiEmbeddingApi CreateEmbedding(string? name = null)
            => Create(name).Embedding;
        public IOpenAiFileApi CreateFile(string? name = null)
            => Create(name).File;
        public IOpenAiFineTuneApi CreateFineTune(string? name = null)
            => Create(name).FineTune;
        public IOpenAiImageApi CreateImage(string? name = null)
            => Create(name).Image;
        public IOpenAiModelApi CreateModel(string? name = null)
            => Create(name).Model;
        public IOpenAiModerationApi CreateModeration(string? name = null)
            => Create(name).Moderation;
    }
}
