using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            var apiKey = context.Configuration["Azure:ApiKey"];
            var resourceName = context.Configuration["Azure:ResourceName"];
            var clientId = context.Configuration["AzureAd:ClientId"];
            var clientSecret = context.Configuration["AzureAd:ClientSecret"];
            var tenantId = context.Configuration["AzureAd:TenantId"];
            services.AddOpenAi(settings =>
            {
                settings.ApiKey = apiKey;
                settings.Azure.ResourceName = resourceName;
                settings.Azure.AppRegistration.ClientId = clientId;
                settings.Azure.AppRegistration.ClientSecret = clientSecret;
                settings.Azure.AppRegistration.TenantId = tenantId;
                settings.Azure
                    .AddDeploymentTextModel("Test", TextModelType.CurieText)
                    .AddDeploymentTextModel("text-davinci-002", TextModelType.DavinciText2)
                    .AddDeploymentEmbeddingModel("Test", EmbeddingModelType.AdaTextEmbedding);
            });
        }
    }
}
