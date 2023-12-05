using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;
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
        public IOpenAi Create(string? integrationName = default)
        {
            integrationName ??= string.Empty;
            if (_openAiApi is OpenAiApi internalImplementation)
                internalImplementation.SetName(integrationName);
            return _openAiApi;
        }
        public IOpenAiAudio CreateAudio(string? integrationName = default)
            => Create(integrationName).Audio;
        public IOpenAiChat CreateChat(string? integrationName = default)
            => Create(integrationName).Chat;
        public IOpenAiEmbedding CreateEmbedding(string? integrationName = default)
            => Create(integrationName).Embedding;
        public IOpenAiFile CreateFile(string? integrationName = default)
            => Create(integrationName).File;
        public IOpenAiFineTune CreateFineTune(string? integrationName = default)
            => Create(integrationName).FineTune;
        public IOpenAiImage CreateImage(string? integrationName = default)
            => Create(integrationName).Image;
        public IOpenAiModel CreateModel(string? integrationName = default)
            => Create(integrationName).Model;
        public IOpenAiModeration CreateModeration(string? integrationName = default)
            => Create(integrationName).Moderation;
        public IOpenAiManagement CreateManagement(string? integrationName = default)
            => Create(integrationName).Management;
        public IOpenAiDeployment CreateDeployment(string? integrationName = default)
            => Create(integrationName).Management.Deployment;
        public IOpenAiBilling CreateBilling(string? integrationName = default)
            => Create(integrationName).Management.Billing;
    }
}
