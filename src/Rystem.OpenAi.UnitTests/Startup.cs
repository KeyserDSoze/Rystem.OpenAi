using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rystem.Test.XUnit;

namespace Rystem.OpenAi.UnitTests
{
    public sealed class Startup : StartupHelper
    {
        protected override string? AppSettingsFileName => "appsettings.test.json";

        protected override bool HasTestHost => false;

        protected override Type? TypeToChooseTheRightAssemblyToRetrieveSecretsForConfiguration => typeof(Startup);

        protected override Type? TypeToChooseTheRightAssemblyWithControllersToMap => null;

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
                    .MapDeployment("text-curie-001", "");
            }, "Azure");
            services
                .AddOpenAi(settings =>
                {
                    settings.ApiKey = azureApiKey2;
                    settings.Azure.ResourceName = resourceName2;
                }, "Azure2");
            return services;
        }
    }
}
