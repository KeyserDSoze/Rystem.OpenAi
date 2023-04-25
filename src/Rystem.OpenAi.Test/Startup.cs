using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class Startup
    {
        private sealed class ForUserSecrets { }

        public void ConfigureHost(IHostBuilder hostBuilder) =>
        hostBuilder
            .ConfigureHostConfiguration(builder => { })
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.test.json")
               .AddUserSecrets<ForUserSecrets>();
            });
        public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
        {
            var apiKey = context.Configuration["OpenAi:ApiKey"];
            services
                .AddOpenAi(settings =>
                {
                    settings.ApiKey = apiKey;
                });
            var azureApiKey = context.Configuration["Azure:ApiKey"];
            var resourceName = context.Configuration["Azure:ResourceName"];
            var clientId = context.Configuration["AzureAd:ClientId"];
            var clientSecret = context.Configuration["AzureAd:ClientSecret"];
            var tenantId = context.Configuration["AzureAd:TenantId"];
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
                    .MapDeploymentChatModel("gpt35turbo", ChatModelType.Gpt35Turbo0301)
                    .MapDeploymentCustomModel("ada001", "text-ada-001");
                settings.Price
                    .SetFineTuneForAda(0.0004M, 0.0016M)
                    .SetAudioForTranslation(0.006M);
            }, "Azure");
            var azureApiKey2 = context.Configuration["Azure2:ApiKey"];
            var resourceName2 = context.Configuration["Azure2:ResourceName"];
            services
                .AddOpenAi(settings =>
                {
                    settings.ApiKey = azureApiKey2;
                    settings.Azure.ResourceName = resourceName2;
                }, "Azure2");
            var result = services.BuildServiceProvider().MapDeploymentsAutomaticallyAsync(true, "Azure").ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Empty(result);
            OpenAiService.Setup(settings =>
            {
                settings.ApiKey = apiKey;
            }, "NoDI");

        }
    }
}
