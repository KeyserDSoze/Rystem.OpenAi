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
    public interface IOpenAiFactory
    {
        IOpenAiApi Create(string? name = default);
        IOpenAiAudioApi CreateAudio(string? name = default);
        IOpenAiChatApi CreateChat(string? name = default);
        IOpenAiCompletionApi CreateCompletion(string? name = default);
        IOpenAiEditApi CreateEdit(string? name = default);
        IOpenAiEmbeddingApi CreateEmbedding(string? name = default);
        IOpenAiFileApi CreateFile(string? name = default);
        IOpenAiFineTuneApi CreateFineTune(string? name = default);
        IOpenAiImageApi CreateImage(string? name = default);
        IOpenAiModelApi CreateModel(string? name = default);
        IOpenAiModerationApi CreateModeration(string? name = default);
    }
}
