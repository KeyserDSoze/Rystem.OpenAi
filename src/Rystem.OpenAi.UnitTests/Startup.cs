using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rystem.PlayFramework.Test.Api;
using Rystem.Test.XUnit;

namespace Rystem.OpenAi.UnitTests
{
    public sealed class Startup : StartupHelper
    {
        protected override string? AppSettingsFileName => "appsettings.test.json";

        protected override bool HasTestHost => true;

        protected override Type? TypeToChooseTheRightAssemblyToRetrieveSecretsForConfiguration => typeof(Startup);

        protected override Type? TypeToChooseTheRightAssemblyWithControllersToMap => typeof(CountryController);

        protected override IServiceCollection ConfigureClientServices(IServiceCollection services, IConfiguration configuration)
        {
            var apiKey = Environment.GetEnvironmentVariable("OpenAiApiKey") ?? configuration["OpenAi:ApiKey"];
            var azureApiKey = Environment.GetEnvironmentVariable("AzureApiKey") ?? configuration["Azure:ApiKey"];
            var resourceName = Environment.GetEnvironmentVariable("AzureResourceName") ?? configuration["Azure:ResourceName"];
            var clientId = Environment.GetEnvironmentVariable("AzureADClientId") ?? configuration["AzureAd:ClientId"];
            var clientSecret = Environment.GetEnvironmentVariable("AzureADClientSecret") ?? configuration["AzureAd:ClientSecret"];
            var tenantId = Environment.GetEnvironmentVariable("AzureADTenantId") ?? configuration["AzureAd:TenantId"];
            var azureApiKey2 = Environment.GetEnvironmentVariable("Azure2ApiKey") ?? configuration["Azure2:ApiKey"];
            var resourceName2 = Environment.GetEnvironmentVariable("Azure2ResourceName") ?? configuration["Azure2:ResourceName"];
            var azureApiKey3 = Environment.GetEnvironmentVariable("Azure3ApiKey") ?? configuration["Azure3:ApiKey"];
            var resourceName3 = Environment.GetEnvironmentVariable("Azure3ResourceName") ?? configuration["Azure3:ResourceName"];
            services.AddLogging(builder => builder.AddConsole());
            services.AddHttpClient("client", x =>
            {
                x.BaseAddress = new Uri("http://localhost");
            });
            OpenAiServiceLocator.Configuration.AddOpenAi(settings =>
                {
                    settings.ApiKey = apiKey;
                }, "NoDI");
            services
                .AddOpenAi(settings =>
                {
                    settings.ApiKey = apiKey;
                });
            services.AddOpenAi(settings =>
            {
                //settings.ApiKey = azureApiKey;
                settings.Version = "2024-08-01-preview";
                settings
                    .UseVersionForChat("2024-08-01-preview");
                settings.Azure.ResourceName = resourceName;
                settings.Azure.AppRegistration.ClientId = clientId;
                settings.Azure.AppRegistration.ClientSecret = clientSecret;
                settings.Azure.AppRegistration.TenantId = tenantId;
                settings
                    .MapDeploymentForEveryRequests(OpenAiType.Chat, "gpt-4");
                settings.DefaultRequestConfiguration.Chat = chatClient =>
                {
                    chatClient.ForceModel("gpt-4");
                };
                settings.PriceBuilder
                    .AddModel("gpt-4",
                    new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                    new OpenAiCost { Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00000125m },
                    new OpenAiCost { Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00001m });
            }, "Azure");
            services
                .AddOpenAi(settings =>
                {
                    settings.ApiKey = azureApiKey2;
                    settings.Azure.ResourceName = resourceName2;
                    settings.Version = "2024-02-01";
                    settings.UseVersionForAudioSpeech("2024-05-01-preview");
                    settings.UseVersionForAudioTranscription("2024-06-01");
                    settings.UseVersionForAudioTranslation("2024-06-01");
                    settings.MapDeploymentForEveryRequests(OpenAiType.AudioSpeech, "tts-hd");
                    settings.MapDeploymentForEveryRequests(OpenAiType.AudioTranscription, "whisper");
                    settings.MapDeploymentForEveryRequests(OpenAiType.AudioTranslation, "whisper");
                    settings.DefaultRequestConfiguration.Chat = chatClient =>
                    {
                        chatClient.ForceModel("gpt-4");
                    };
                }, "Azure2");
            services
               .AddOpenAi(settings =>
               {
                   settings.ApiKey = azureApiKey3;
                   settings.Azure.ResourceName = resourceName3;
                   settings.Version = "2024-02-01";
                   settings.UseVersionForAudioSpeech("2024-05-01-preview");
                   settings.UseVersionForAudioTranscription("2024-06-01");
                   settings.UseVersionForAudioTranslation("2024-06-01");
                   settings.MapDeploymentForEveryRequests(OpenAiType.AudioSpeech, "tts-hd");
                   settings.MapDeploymentForEveryRequests(OpenAiType.AudioTranscription, "whisper");
                   settings.MapDeploymentForEveryRequests(OpenAiType.AudioTranslation, "whisper");
                   settings.DefaultRequestConfiguration.Chat = chatClient =>
                   {
                       chatClient.ForceModel("gpt-4");
                   };
               }, "Azure3")
               .ConfigureOpenAiLogging(x =>
               {
                   x.Request = LogLevel.Information;
                   x.Error = LogLevel.Error;
               });
            return services;
        }
        protected override ValueTask ConfigureServerServicesAsync(IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices(configuration);
            services.AddLogging(builder => builder.AddConsole());
            return ValueTask.CompletedTask;
        }
        protected override ValueTask ConfigureServerMiddlewareAsync(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            applicationBuilder.UseMiddlewares();
            return ValueTask.CompletedTask;
        }
    }
}
