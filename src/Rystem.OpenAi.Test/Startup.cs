using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rystem.OpenAi.Test.Functions;
using Rystem.OpenAi.Test.Logger;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class Startup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2094:Classes should not be empty", Justification = "It's necessary to inject secrets in Dependency injection settings.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Test purposes.")]
        private sealed class ForUserSecrets { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "It's necessary to have this method as a non-static method because the dependency injection package needs a non-static method.")]
        public void ConfigureHost(IHostBuilder hostBuilder) =>
        hostBuilder
            .ConfigureHostConfiguration(builder => { })
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.test.json")
               .AddUserSecrets<ForUserSecrets>()
               .AddEnvironmentVariables();
            });
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "It's necessary to have this method as a non-static method because the dependency injection package needs a non-static method.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "Test purposes.")]
        public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
        {
            var apiKey = Environment.GetEnvironmentVariable("OpenAiApiKey") ?? context.Configuration["OpenAi:ApiKey"];
            var azureApiKey = Environment.GetEnvironmentVariable("AzureApiKey") ?? context.Configuration["Azure:ApiKey"];
            var resourceName = Environment.GetEnvironmentVariable("AzureResourceName") ?? context.Configuration["Azure:ResourceName"];
            var clientId = Environment.GetEnvironmentVariable("AzureADClientId") ?? context.Configuration["AzureAd:ClientId"];
            var clientSecret = Environment.GetEnvironmentVariable("AzureADClientSecret") ?? context.Configuration["AzureAd:ClientSecret"];
            var tenantId = Environment.GetEnvironmentVariable("AzureADTenantId") ?? context.Configuration["AzureAd:TenantId"];
            var azureApiKey2 = Environment.GetEnvironmentVariable("Azure2ApiKey") ?? context.Configuration["Azure2:ApiKey"];
            var resourceName2 = Environment.GetEnvironmentVariable("Azure2ResourceName") ?? context.Configuration["Azure2:ResourceName"];
            services.AddSingleton<CustomLoggerMemory>();
            services.AddTransient(typeof(ILogger<>), typeof(CustomLogger<>));
            services
                .AddOpenAi(settings =>
                {
                    settings.ApiKey = apiKey;
                });
            services.AddOpenAi(settings =>
            {
                settings.ApiKey = azureApiKey;
                settings
                    .UseVersionForChat("2023-03-15-preview");
                settings.Azure.ResourceName = resourceName;
                settings.Azure.AppRegistration.ClientId = clientId;
                settings.Azure.AppRegistration.ClientSecret = clientSecret;
                settings.Azure.AppRegistration.TenantId = tenantId;
                settings.Azure
                    .MapDeploymentTextModel("text-curie-001", TextModelType.CurieText)
                    .MapDeploymentTextModel("text-davinci-003", TextModelType.DavinciText3)
                    .MapDeploymentEmbeddingModel("OpenAiDemoModel", EmbeddingModelType.AdaTextEmbedding)
                    .MapDeploymentChatModel("gpt35turbo", ChatModelType.Gpt35Turbo)
                    .MapDeploymentCustomModel("deployment-a074644e38f642658a73a3189f64bc1b", "text-ada-001");
                settings.Price
                    .SetFineTuneForAda(0.0004M, 0.0016M)
                    .SetAudioForTranslation(0.006M);
            }, "Azure");
            services
                .AddOpenAi(settings =>
                {
                    settings.ApiKey = azureApiKey2;
                    settings.Azure.ResourceName = resourceName2;
                }, "Azure2");
            var tasks = new List<ValueTask<List<AutomaticallyDeploymentResult>>>
            {
                services.BuildServiceProvider().MapDeploymentsAutomaticallyAsync(true, "Azure"),
                OpenAiService.Instance.AddOpenAi(settings =>
                {
                    settings.ApiKey = apiKey;
                }, "NoDI")
                .AddOpenAi(settings =>
                {
                    settings.ApiKey = azureApiKey;
                    settings
                        .UseVersionForChat("2023-03-15-preview");
                    settings.Azure.ResourceName = resourceName;
                    settings.Azure.AppRegistration.ClientId = clientId;
                    settings.Azure.AppRegistration.ClientSecret = clientSecret;
                    settings.Azure.AppRegistration.TenantId = tenantId;
                    settings.Azure
                        .MapDeploymentTextModel("text-curie-001", TextModelType.CurieText)
                        .MapDeploymentTextModel("text-davinci-003", TextModelType.DavinciText3)
                        .MapDeploymentEmbeddingModel("OpenAiDemoModel", EmbeddingModelType.AdaTextEmbedding)
                        .MapDeploymentChatModel("gpt35turbo", ChatModelType.Gpt35Turbo)
                        .MapDeploymentCustomModel("ada001", "text-ada-001");
                    settings.Price
                        .SetFineTuneForAda(0.0004M, 0.0016M)
                        .SetAudioForTranslation(0.006M);
                }, "Azure")
                .MapDeploymentsAutomaticallyAsync(true, "Azure")
            };
            services
                .AddOpenAiChatFunction<WeatherFunction>()
                .AddOpenAiChatFunction<AirplaneFunction>()
                .AddOpenAiChatFunction<GroceryFunction>()
                .AddOpenAiChatFunction<MapFunction>()
                .AddOpenAiChatFunction<NullFunction>();
            var results = Task.WhenAll(tasks.Select(x => x.AsTask())).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.NotEmpty(results);
        }
    }
}
