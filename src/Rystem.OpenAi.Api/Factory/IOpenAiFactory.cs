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
    public interface IOpenAiFactory
    {
        IOpenAi Create(string? integrationName = default);
        IOpenAiAudio CreateAudio(string? integrationName = default);
        IOpenAiChat CreateChat(string? integrationName = default);
        IOpenAiEmbedding CreateEmbedding(string? integrationName = default);
        IOpenAiFile CreateFile(string? integrationName = default);
        IOpenAiFineTune CreateFineTune(string? integrationName = default);
        IOpenAiImage CreateImage(string? integrationName = default);
        IOpenAiModel CreateModel(string? integrationName = default);
        IOpenAiModeration CreateModeration(string? integrationName = default);
        IOpenAiManagement CreateManagement(string? integrationName = default);
        IOpenAiDeployment CreateDeployment(string? integrationName = default);
        IOpenAiBilling CreateBilling(string? integrationName = default);
    }
}
