using System;
using System.Collections.Generic;
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
    public sealed class OpenAiService : IOpenAiServiceSetupNoDependencyInjection, IOpenAiFactoryNoDependencyInjection
    {
        private static readonly OpenAiService s_openAiService = new OpenAiService();
        public static IOpenAiServiceSetupNoDependencyInjection Instance => s_openAiService;
        public static IOpenAiFactoryNoDependencyInjection Factory => s_openAiService;
        private OpenAiService() { }
        private readonly IServiceCollection _services = new ServiceCollection();
        private IServiceProvider? _serviceProvider;
        public IOpenAiServiceSetupNoDependencyInjection AddOpenAi(Action<OpenAiSettings> settings, string? integrationName = default)
        {
            _services.AddOpenAi(settings, integrationName);
            return this;
        }
        public ValueTask<List<AutomaticallyDeploymentResult>> MapDeploymentsAutomaticallyAsync(bool forceDeploy = false, params string[] integrationNames)
        {
            _serviceProvider ??= _services.BuildServiceProvider();
            return _serviceProvider.MapDeploymentsAutomaticallyAsync(forceDeploy, integrationNames);
        }
        public IOpenAi Create(string? integrationName = default)
        {
            _serviceProvider ??= _services.BuildServiceProvider();
            var factory = _serviceProvider.GetService<IOpenAiFactory>()!;
            if (integrationName == default)
                integrationName = string.Empty;
            return factory.Create(integrationName);
        }
        public IOpenAiAudio CreateAudio(string? integrationName = null)
            => Create(integrationName).Audio;
        public IOpenAiChat CreateChat(string? integrationName = null)
            => Create(integrationName).Chat;
        public IOpenAiCompletion CreateCompletion(string? integrationName = null)
            => Create(integrationName).Completion;
        public IOpenAiEdit CreateEdit(string? integrationName = null)
            => Create(integrationName).Edit;
        public IOpenAiEmbedding CreateEmbedding(string? integrationName = null)
            => Create(integrationName).Embedding;
        public IOpenAiFile CreateFile(string? integrationName = null)
            => Create(integrationName).File;
        public IOpenAiFineTune CreateFineTune(string? integrationName = null)
            => Create(integrationName).FineTune;
        public IOpenAiImage CreateImage(string? integrationName = null)
            => Create(integrationName).Image;
        public IOpenAiModel CreateModel(string? integrationName = null)
            => Create(integrationName).Model;
        public IOpenAiModeration CreateModeration(string? integrationName = null)
            => Create(integrationName).Moderation;
        public IOpenAiManagement CreateManagement(string? integrationName = null)
            => Create(integrationName).Management;
        public IOpenAiBilling CreateBilling(string? integrationName = null)
            => Create(integrationName).Management.Billing;
        public IOpenAiDeployment CreateDeployment(string? integrationName = null)
            => Create(integrationName).Management.Deployment;
        public IOpenAiUtility Utility()
        {
            _serviceProvider ??= _services.BuildServiceProvider();
            return _serviceProvider.GetService<IOpenAiUtility>()!;
        }
    }
}
