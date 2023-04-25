using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
    public static class OpenAiService
    {
        private static readonly IServiceCollection s_services = new ServiceCollection();
        private static IServiceProvider? s_serviceProvider;
        public static void Setup(Action<OpenAiSettings> settings, string? integrationName = default)
        {
            s_services.AddOpenAi(settings, integrationName);
        }
        public static ValueTask<bool> MapDeploymentsAutomaticallyAsync()
        {
            s_serviceProvider ??= s_services.BuildServiceProvider();
            return s_serviceProvider.MapDeploymentsAutomaticallyAsync();
        }
        public static IOpenAi Create(string? name = default)
        {
            s_serviceProvider ??= s_services.BuildServiceProvider();
            var factory = s_serviceProvider.GetService<IOpenAiFactory>()!;
            if (name == default)
                name = string.Empty;
            return factory.Create(name);
        }
        public static IOpenAiAudio CreateAudio(string? name = null)
            => Create(name).Audio;
        public static IOpenAiChat CreateChat(string? name = null)
            => Create(name).Chat;
        public static IOpenAiCompletion CreateCompletion(string? name = null)
            => Create(name).Completion;
        public static IOpenAiEdit CreateEdit(string? name = null)
            => Create(name).Edit;
        public static IOpenAiEmbedding CreateEmbedding(string? name = null)
            => Create(name).Embedding;
        public static IOpenAiFile CreateFile(string? name = null)
            => Create(name).File;
        public static IOpenAiFineTune CreateFineTune(string? name = null)
            => Create(name).FineTune;
        public static IOpenAiImage CreateImage(string? name = null)
            => Create(name).Image;
        public static IOpenAiModel CreateModel(string? name = null)
            => Create(name).Model;
        public static IOpenAiModeration CreateModeration(string? name = null)
            => Create(name).Moderation;
        public static IOpenAiManagement CreateManagement(string? name = null)
            => Create(name).Management;
        public static IOpenAiUtility Utility()
        {
            s_serviceProvider ??= s_services.BuildServiceProvider();
            return s_serviceProvider.GetService<IOpenAiUtility>()!;
        }
    }
}
