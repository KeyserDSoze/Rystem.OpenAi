using System;
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
    public sealed class DefaultRequestConfiguration
    {
        public Action<IOpenAiChat>? Chat { get; set; }
        public Action<IOpenAiAudio>? Audio { get; set; }
        public Action<IOpenAiSpeech>? Speech { get; set; }
        public Action<IOpenAiEmbedding>? Embeddings { get; set; }
        public Action<IOpenAiFineTune>? FineTune { get; set; }
        public Action<IOpenAiFile>? File { get; set; }
        public Action<IOpenAiImage>? Image { get; set; }
        public Action<IOpenAiModel>? Model { get; set; }
        public Action<IOpenAiModeration>? Moderation { get; set; }
        public Action<IOpenAiManagement>? Management { get; set; }
        public Action<IOpenAiAssistant>? Assistant { get; set; }
        public Action<IOpenAiThread>? Thread { get; set; }
        public Action<IOpenAiMessage>? Message { get; set; }
        public Action<IOpenAiRun>? Run { get; set; }
        public Action<IOpenAiVectorStore>? VectorStore { get; set; }
    }
}
