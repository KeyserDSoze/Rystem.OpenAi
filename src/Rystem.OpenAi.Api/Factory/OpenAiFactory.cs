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
    internal sealed class OpenAiFactory : IOpenAiFactory
    {
        private readonly IOpenAi _openAiApi;

        public OpenAiFactory(IOpenAi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        public IOpenAi Create(string? name = default)
        {
            name ??= string.Empty;
            if (_openAiApi is OpenAiApi internalImplementation)
                internalImplementation.SetName(name);
            return _openAiApi;
        }
        public IOpenAiAudio CreateAudio(string? name = null)
            => Create(name).Audio;
        public IOpenAiChat CreateChat(string? name = null)
            => Create(name).Chat;
        public IOpenAiCompletion CreateCompletion(string? name = null)
            => Create(name).Completion;
        public IOpenAiEdit CreateEdit(string? name = null)
            => Create(name).Edit;
        public IOpenAiEmbedding CreateEmbedding(string? name = null)
            => Create(name).Embedding;
        public IOpenAiFile CreateFile(string? name = null)
            => Create(name).File;
        public IOpenAiFineTune CreateFineTune(string? name = null)
            => Create(name).FineTune;
        public IOpenAiImage CreateImage(string? name = null)
            => Create(name).Image;
        public IOpenAiModel CreateModel(string? name = null)
            => Create(name).Model;
        public IOpenAiModeration CreateModeration(string? name = null)
            => Create(name).Moderation;
        public IOpenAiManagement CreateManagement(string? name = default)
            => Create(name).Management;
    }
}
