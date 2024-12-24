using Rystem.OpenAi.Assistant;
using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Embedding;
using Rystem.OpenAi.Files;
using Rystem.OpenAi.FineTune;
using Rystem.OpenAi.Image;
using Rystem.OpenAi.Models;
using Rystem.OpenAi.Moderation;

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
        IOpenAiAssistant Assistant { get; }
        IOpenAiThread Thread { get; }
    }
}
