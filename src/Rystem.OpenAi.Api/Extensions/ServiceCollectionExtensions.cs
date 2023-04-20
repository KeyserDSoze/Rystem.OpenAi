using System;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Extensions.Http;
using Rystem.OpenAi;
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

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3928:Parameter names used into ArgumentException constructors should match an existing one ", Justification = "The parameter is in the root of the setting class.")]
        public static IServiceCollection AddOpenAi(this IServiceCollection services, Action<OpenAiSettings> settings, string? name = default)
        {
            var openAiSettings = new OpenAiSettings();
            settings.Invoke(openAiSettings);
            if (openAiSettings.ApiKey == null && !openAiSettings.Azure.HasAnotherKindOfAuthentication)
                throw new ArgumentNullException($"{nameof(OpenAiSettings.ApiKey)} is empty.");

            services
                .TryAddTransient<IOpenAiFactory, OpenAiFactory>();

            services.AddSingleton(new OpenAiConfiguration(openAiSettings, name));
            var httpClientBuilder = services.AddHttpClient(OpenAiSettings.HttpClientName, client =>
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
                client.Timeout = TimeSpan.FromMinutes(10);
            });
            if (openAiSettings.RetryPolicy)
            {
                var defaultPolicy = openAiSettings.CustomRetryPolicy ?? Policy<HttpResponseMessage>
                   .Handle<HttpRequestException>()
                   .OrTransientHttpError()
                   .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromSeconds(10), 10, TimeSpan.FromSeconds(15));
                httpClientBuilder
                     .AddPolicyHandler(defaultPolicy);
            }
            if (!OpenAiPriceList.Instance.Prices.ContainsKey(name ?? string.Empty))
                OpenAiPriceList.Instance.Prices.Add(name ?? string.Empty, openAiSettings.Price);
            else
                OpenAiPriceList.Instance.Prices[name ?? string.Empty] = openAiSettings.Price;
            services.TryAddSingleton(OpenAiPriceList.Instance);
            services
                .TryAddSingleton<IOpenAiUtility, OpenAiUtility>();
            services
                .TryAddSingleton<IOpenAiTokenizer, OpenAiTokenizer>();
            services
                .TryAddSingleton<IOpenAiCost, OpenAiCost>();
            services
                .TryAddTransient<IOpenAi, OpenAiApi>();
            services
                .TryAddTransient<IOpenAiEmbedding, OpenAiEmbedding>();
            services
                .TryAddTransient<IOpenAiFile, OpenAiFile>();
            services
                .TryAddTransient<IOpenAiAudio, OpenAiAudio>();
            services
                .TryAddTransient<IOpenAiModel, OpenAiModel>();
            services
                .TryAddTransient<IOpenAiModeration, OpenAiModeration>();
            services
                .TryAddTransient<IOpenAiImage, OpenAiImage>();
            services
                .TryAddTransient<IOpenAiFineTune, OpenAiFineTune>();
            services
                .TryAddTransient<IOpenAiEdit, OpenAiEdit>();
            services
                .TryAddTransient<IOpenAiChat, OpenAiChat>();
            services
                .TryAddTransient<IOpenAiCompletion, OpenAiCompletion>();
            services
                .TryAddTransient<IOpenAiManagement, OpenAiManagement>();

            services
               .TryAddTransient<IOpenAiApi, OpenAiApi>();
            services
                .TryAddTransient<IOpenAiEmbeddingApi, OpenAiEmbedding>();
            services
                .TryAddTransient<IOpenAiFileApi, OpenAiFile>();
            services
                .TryAddTransient<IOpenAiAudioApi, OpenAiAudio>();
            services
                .TryAddTransient<IOpenAiModelApi, OpenAiModel>();
            services
                .TryAddTransient<IOpenAiModerationApi, OpenAiModeration>();
            services
                .TryAddTransient<IOpenAiImageApi, OpenAiImage>();
            services
                .TryAddTransient<IOpenAiFineTuneApi, OpenAiFineTune>();
            services
                .TryAddTransient<IOpenAiEditApi, OpenAiEdit>();
            services
                .TryAddTransient<IOpenAiChatApi, OpenAiChat>();
            services
                .TryAddTransient<IOpenAiCompletionApi, OpenAiCompletion>();
            services
                .TryAddTransient<IOpenAiManagementApi, OpenAiManagement>();
            return services;
        }
    }
}
