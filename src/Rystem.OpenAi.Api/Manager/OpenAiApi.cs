using System.Collections.Generic;
using System.Net.Http;
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
    internal sealed class OpenAiApi : OpenAiBase, IOpenAi
    {
        public IOpenAiModel Model { get; }
        public IOpenAiImage Image { get; }
        public IOpenAiEmbedding Embedding { get; }
        public IOpenAiFile File { get; }
        public IOpenAiModeration Moderation { get; }
        public IOpenAiAudio Audio { get; }
        public IOpenAiFineTune FineTune { get; }
        public IOpenAiChat Chat { get; }
        public IOpenAiManagement Management { get; }

        public OpenAiApi(
            IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility,
            IOpenAiEmbedding embeddingApi,
            IOpenAiModel modelApi,
            IOpenAiFile fileApi,
            IOpenAiImage imageApi,
            IOpenAiModeration moderationApi,
            IOpenAiAudio audioApi,
            IOpenAiFineTune fineTuneApi,
            IOpenAiChat chatApi,
            IOpenAiManagement managementApi) : base(httpClientFactory, configurations, utility)
        {
            Embedding = embeddingApi;
            SetAiBase(Embedding);

            Model = modelApi;
            SetAiBase(Model);

            File = fileApi;
            SetAiBase(File);

            Image = imageApi;
            SetAiBase(Image);

            Moderation = moderationApi;
            SetAiBase(Moderation);

            Audio = audioApi;
            SetAiBase(Audio);

            FineTune = fineTuneApi;
            SetAiBase(FineTune);

            Chat = chatApi;
            SetAiBase(Chat);

            Management = managementApi;
            SetAiBase(Management);
        }
    }
}
