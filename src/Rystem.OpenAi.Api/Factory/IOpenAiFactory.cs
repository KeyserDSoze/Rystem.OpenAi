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
    public interface IOpenAiFactory
    {
        IOpenAi Create(string? name = default);
        IOpenAiAudio CreateAudio(string? name = default);
        IOpenAiChat CreateChat(string? name = default);
        IOpenAiCompletion CreateCompletion(string? name = default);
        IOpenAiEdit CreateEdit(string? name = default);
        IOpenAiEmbedding CreateEmbedding(string? name = default);
        IOpenAiFile CreateFile(string? name = default);
        IOpenAiFineTune CreateFineTune(string? name = default);
        IOpenAiImage CreateImage(string? name = default);
        IOpenAiModel CreateModel(string? name = default);
        IOpenAiModeration CreateModeration(string? name = default);
        IOpenAiManagement CreateManagement(string? name = default);
    }
}
