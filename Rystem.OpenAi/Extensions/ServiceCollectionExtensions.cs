using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Polly;
using Polly.Extensions.Http;
using Rystem.OpenAi;
using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Embedding;
using Rystem.OpenAi.Files;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenAi(this IServiceCollection services,
            Action<OpenAiSettings> settings,
            string? integrationName = default)
        {
            var openAiSettings = new OpenAiSettings();
            settings.Invoke(openAiSettings);
            if (openAiSettings.ApiKey == null && !openAiSettings.Azure.HasAnotherKindOfAuthentication)
                throw new ArgumentNullException($"{nameof(OpenAiSettings.ApiKey)} is empty.");
            services.AddFactory<IOpenAi, OpenAi>(integrationName);
            services.AddFactory(new OpenAiConfiguration(openAiSettings, integrationName), integrationName, ServiceLifetime.Singleton);
            services.AddFactory<IOpenAiPriceService>(new PriceService(openAiSettings.PriceBuilder.Prices), integrationName, ServiceLifetime.Singleton);
            var httpClientBuilder = services.AddHttpClient($"{OpenAiSettings.HttpClientName}-{integrationName}", client =>
            {
                if (openAiSettings.Azure.HasConfiguration)
                {
                    if (!openAiSettings.Azure.HasAnotherKindOfAuthentication)
                        client.DefaultRequestHeaders.Add("api-key", openAiSettings.ApiKey);
                }
                else
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiSettings.ApiKey);
                if (!string.IsNullOrEmpty(openAiSettings.OrganizationName))
                    client.DefaultRequestHeaders.Add("OpenAI-Organization", openAiSettings.OrganizationName);
                if (!string.IsNullOrEmpty(openAiSettings.ProjectId))
                    client.DefaultRequestHeaders.Add("OpenAI-Project", openAiSettings.ProjectId);
                client.Timeout = TimeSpan.FromMinutes(10);
            });
            if (openAiSettings.RetryPolicy)
            {
                var defaultPolicy = openAiSettings.CustomRetryPolicy ?? Policy<HttpResponseMessage>
                   .Handle<HttpRequestException>()
                   .OrTransientHttpError()
                   .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(0.5));
                httpClientBuilder
                     .AddPolicyHandler(defaultPolicy);
            }
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
                .AddFactory<IOpenAiModel, OpenAiModelService>(integrationName, ServiceLifetime.Transient);
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
            //services
            //    .AddFactory<IOpenAiBilling, OpenAiBilling>(integrationName, ServiceLifetime.Transient);
            //services
            //    .AddFactory<IOpenAiDeployment, OpenAiDeployment>(integrationName, ServiceLifetime.Transient);
            return services;
        }
    }
}
