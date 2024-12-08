using System;
using Microsoft.Extensions.DependencyInjection;
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
    public sealed class OpenAiServiceLocator : IOpenAiServiceLocatorConfigurator, IOpenAiServiceLocator
    {
        private static readonly OpenAiServiceLocator s_openAiService = new();
        public static IOpenAiServiceLocator Instance => s_openAiService;
        public static IOpenAiServiceLocatorConfigurator Configuration => s_openAiService;
        private OpenAiServiceLocator() { }
        private readonly IServiceCollection _services = new ServiceCollection();
        public IServiceCollection Services => _services;
        private IServiceProvider? _serviceProvider;
        public IOpenAiServiceLocatorConfigurator AddOpenAi(Action<OpenAiSettings> settings, string? integrationName = default)
        {
            _services.AddOpenAi(settings, integrationName);
            return this;
        }
        public IOpenAiServiceLocatorConfigurator AddFurtherService<TService, TImplementation>(ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            _services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
            return this;
        }
        public IOpenAi Create(string? integrationName = default)
        {
            _serviceProvider ??= _services.BuildServiceProvider();
            var factory = _serviceProvider.GetService<IFactory<IOpenAi>>()!;
            if (integrationName == default)
                integrationName = string.Empty;
            return factory.Create(integrationName)!;
        }
        public IOpenAiAudio CreateAudio(string? integrationName = null)
            => Create(integrationName).Audio;
        public IOpenAiSpeech CreateSpeech(string? integrationName = null)
            => Create(integrationName).Speech;
        public IOpenAiChat CreateChat(string? integrationName = null)
            => Create(integrationName).Chat;
        public IOpenAiEmbedding CreateEmbedding(string? integrationName = null)
            => Create(integrationName).Embeddings;
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
        public IOpenAiUtility Utility()
        {
            _serviceProvider ??= _services.BuildServiceProvider();
            return _serviceProvider.GetService<IOpenAiUtility>()!;
        }
    }
}
