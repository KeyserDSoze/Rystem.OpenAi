using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Polly;
using Polly.Extensions.Http;
using Rystem.OpenAi;
using Rystem.OpenAi.Assistant;
using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Embedding;
using Rystem.OpenAi.Files;
using Rystem.OpenAi.FineTune;
using Rystem.OpenAi.Image;
using Rystem.OpenAi.Models;
using Rystem.OpenAi.Moderation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private const string ApiKeyHeaderNameAzure = "api-key";
        private const string AuthorizationScheme = "Bearer";
        private const string OpenAiOrganizationHeader = "OpenAI-Organization";
        private const string OpenAiProject = "OpenAI-Project";

        public static IServiceCollection AddOpenAi(this IServiceCollection services,
            Action<OpenAiSettings> settings,
            string? integrationName = default)
        {
            var openAiSettings = new OpenAiSettings();
            settings.Invoke(openAiSettings);
            if (openAiSettings.ApiKey == null && !openAiSettings.Azure.HasAnotherKindOfAuthentication)
                throw new ArgumentNullException(openAiSettings.ApiKey, $"Api key is empty.");
            services.AddFactory<IOpenAi, OpenAi>(integrationName);
            services.AddFactory(new OpenAiConfiguration(openAiSettings, integrationName), integrationName, ServiceLifetime.Singleton);
            services.AddFactory<IOpenAiPriceService>(new PriceService(openAiSettings.PriceBuilder.Prices), integrationName, ServiceLifetime.Singleton);
            IAsyncPolicy<HttpResponseMessage>? policy = null;
            if (openAiSettings.RetryPolicy)
            {
                policy = openAiSettings.CustomRetryPolicy ?? Policy<HttpResponseMessage>
                   .Handle<HttpRequestException>()
                   .OrTransientHttpError()
                   .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(0.5));
            }
            services.AddFactory((serviceProvider, context) =>
            {
                var client = new HttpClient();
                var clientWrapper = new HttpClientWrapper() { Client = client, Policy = policy };
                if (openAiSettings.Azure.HasConfiguration)
                {
                    if (!openAiSettings.Azure.HasAnotherKindOfAuthentication)
                        client.DefaultRequestHeaders.Add(ApiKeyHeaderNameAzure, openAiSettings.ApiKey);
                }
                else
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, openAiSettings.ApiKey);
                if (!string.IsNullOrEmpty(openAiSettings.OrganizationName))
                    client.DefaultRequestHeaders.Add(OpenAiOrganizationHeader, openAiSettings.OrganizationName);
                if (!string.IsNullOrEmpty(openAiSettings.ProjectId))
                    client.DefaultRequestHeaders.Add(OpenAiProject, openAiSettings.ProjectId);
                client.Timeout = TimeSpan.FromMinutes(10);
                return clientWrapper;
            }, $"{OpenAiSettings.HttpClientName}_{integrationName ?? string.Empty}");
            services.
                AddFactory<DefaultServices>(integrationName);
            services
                .AddFactory<IOpenAiUtility, OpenAiUtility>(integrationName, ServiceLifetime.Singleton);
            services
                .AddFactory<IOpenAiTokenizer, OpenAiTokenizer>(integrationName, ServiceLifetime.Singleton);
            services
                .AddFactory<IOpenAiEmbedding, OpenAiEmbedding>(integrationName, ServiceLifetime.Transient);
            services
                .AddFactory<IOpenAiFile, OpenAiFile>(integrationName, ServiceLifetime.Transient);
            services
                .AddFactory<IOpenAiAudio, OpenAiAudio>(integrationName, ServiceLifetime.Transient);
            services
                .AddFactory<IOpenAiSpeech, OpenAiSpeech>(integrationName, ServiceLifetime.Transient);
            services
                .AddFactory<IOpenAiModel, OpenAiModel>(integrationName, ServiceLifetime.Transient);
            services
                .AddFactory<IOpenAiModeration, OpenAiModeration>(integrationName, ServiceLifetime.Transient);
            services
                .AddFactory<IOpenAiImage, OpenAiImage>(integrationName, ServiceLifetime.Transient);
            services
                .AddFactory<IOpenAiFineTune, OpenAiFineTune>(integrationName, ServiceLifetime.Transient);
            services
                .AddFactory<IOpenAiChat, OpenAiChat>(integrationName, ServiceLifetime.Transient);
            services
                .AddFactory<IOpenAiManagement, OpenAiManagement>(integrationName, ServiceLifetime.Transient);
            services
                .AddFactory<IOpenAiAssistant, OpenAiAssistant>(integrationName, ServiceLifetime.Transient);
            services
                .AddFactory<IOpenAiThread, OpenAiThread>(integrationName, ServiceLifetime.Transient);
            //services
            //    .AddFactory<IOpenAiBilling, OpenAiBilling>(integrationName, ServiceLifetime.Transient);
            //services
            //    .AddFactory<IOpenAiDeployment, OpenAiDeployment>(integrationName, ServiceLifetime.Transient);
            return services;
        }
    }
}
