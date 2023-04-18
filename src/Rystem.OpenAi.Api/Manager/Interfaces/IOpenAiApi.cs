using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Completion;
using Rystem.OpenAi.Edit;
using Rystem.OpenAi.Embedding;
using Rystem.OpenAi.Files;
using Rystem.OpenAi.FineTune;
using Rystem.OpenAi.Image;
using Rystem.OpenAi;
using Rystem.OpenAi.Moderation;
using Rystem.OpenAi.Management;

namespace Rystem.OpenAi
{
    public interface IOpenAiApi
    {
        IOpenAiModelApi Model { get; }
        IOpenAiFileApi File { get; }
        IOpenAiFineTuneApi FineTune { get; }
        IOpenAiChatApi Chat { get; }
        IOpenAiEditApi Edit { get; }
        IOpenAiCompletionApi Completion { get; }
        IOpenAiImageApi Image { get; }
        IOpenAiEmbeddingApi Embedding { get; }
        IOpenAiModerationApi Moderation { get; }
        IOpenAiAudioApi Audio { get; }
        IOpenAiManagementApi Management { get; }
    }
}
