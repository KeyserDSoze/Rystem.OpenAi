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
    public interface IOpenAi
    {
        IOpenAiModel Model { get; }
        IOpenAiFile File { get; }
        IOpenAiFineTune FineTune { get; }
        IOpenAiChat Chat { get; }
        IOpenAiImage Image { get; }
        IOpenAiEmbedding Embedding { get; }
        IOpenAiModeration Moderation { get; }
        IOpenAiAudio Audio { get; }
        IOpenAiManagement Management { get; }
    }
}
