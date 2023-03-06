using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Completion;
using Rystem.OpenAi.Edit;
using Rystem.OpenAi.Embedding;
using Rystem.OpenAi.File;
using Rystem.OpenAi.FineTune;
using Rystem.OpenAi.Image;
using Rystem.OpenAi.Models;
using Rystem.OpenAi.Moderation;

namespace Rystem.OpenAi
{
    public interface IOpenAiApiFactory
    {
        IOpenAiApi CreateApi();
    }
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
    }
}
