using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Rystem.OpenAi.Moderation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3928:Parameter names used into ArgumentException constructors should match an existing one ", Justification = "The parameter is in the root of the setting class.")]
        public static IServiceCollection AddOpenAi(this IServiceCollection services, Action<OpenAiSettings> settings)
        {
            var openAiSettings = new OpenAiSettings();
            settings.Invoke(openAiSettings);
            if (openAiSettings.ApiKey == null && !openAiSettings.Azure.HasAnotherKindOfAuthentication)
                throw new ArgumentNullException($"{nameof(OpenAiSettings.ApiKey)} is empty.");

            services.AddSingleton(new OpenAiConfiguration(openAiSettings));
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
            services
                .AddSingleton<IOpenAiUtility, OpenAiUtility>()
                .AddScoped<IOpenAiApi, OpenAiApi>()
                .AddScoped<IOpenAiEmbeddingApi, OpenAiEmbeddingApi>()
                .AddScoped<IOpenAiFileApi, OpenAiFileApi>()
                .AddScoped<IOpenAiAudioApi, OpenAiAudioApi>()
                .AddScoped<IOpenAiModelApi, OpenAiModelApi>()
                .AddScoped<IOpenAiModerationApi, OpenAiModerationApi>()
                .AddScoped<IOpenAiImageApi, OpenAiImageApi>()
                .AddScoped<IOpenAiFineTuneApi, OpenAiFineTuneApi>()
                .AddScoped<IOpenAiEditApi, OpenAiEditApi>()
                .AddScoped<IOpenAiChatApi, OpenAiChatApi>()
                .AddScoped<IOpenAiCompletionApi, OpenAiCompletionApi>();
            return services;
        }
    }
}
