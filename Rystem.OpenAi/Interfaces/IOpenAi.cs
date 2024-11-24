using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Embedding;

namespace Rystem.OpenAi
{
    public interface IOpenAi
    {
        IOpenAiAudio Audio { get; }
        IOpenAiChat Chat { get; }
        IOpenAiEmbedding Embeddings { get; }
        IOpenAiFineTune FineTune { get; }
        IOpenAiFile File { get; }
        IOpenAiImage Image { get; }
        IOpenAiModel Model { get; }
        IOpenAiModeration Moderation { get; }
        IOpenAiManagement Management { get; }
        IOpenAiSpeech Speech { get; }
    }
}
