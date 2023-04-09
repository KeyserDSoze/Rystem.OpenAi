using System;
using Microsoft.Extensions.DependencyInjection;
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
    public static class OpenAiService
    {
        private static readonly IServiceCollection s_services = new ServiceCollection();
        private static IServiceProvider? s_serviceProvider;
        public static void Setup(Action<OpenAiSettings> settings, string? name = default)
        {
            s_services.AddOpenAi(settings, name);
        }
        public static IOpenAiApi Create(string? name = default)
        {
            s_serviceProvider ??= s_services.BuildServiceProvider();
            var factory = s_serviceProvider.GetService<IOpenAiFactory>()!;
            if (name == default)
                name = string.Empty;
            return factory.Create(name);
        }
        public static IOpenAiAudioApi CreateAudio(string? name = null)
            => Create(name).Audio;
        public static IOpenAiChatApi CreateChat(string? name = null)
            => Create(name).Chat;
        public static IOpenAiCompletionApi CreateCompletion(string? name = null)
            => Create(name).Completion;
        public static IOpenAiEditApi CreateEdit(string? name = null)
            => Create(name).Edit;
        public static IOpenAiEmbeddingApi CreateEmbedding(string? name = null)
            => Create(name).Embedding;
        public static IOpenAiFileApi CreateFile(string? name = null)
            => Create(name).File;
        public static IOpenAiFineTuneApi CreateFineTune(string? name = null)
            => Create(name).FineTune;
        public static IOpenAiImageApi CreateImage(string? name = null)
            => Create(name).Image;
        public static IOpenAiModelApi CreateModel(string? name = null)
            => Create(name).Model;
        public static IOpenAiModerationApi CreateModeration(string? name = null)
            => Create(name).Moderation;
        public static IOpenAiUtility Utility()
        {
            s_serviceProvider ??= s_services.BuildServiceProvider();
            return s_serviceProvider.GetService<IOpenAiUtility>()!;
        }
    }
}
