using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rystem.OpenAi.UnitTests.Services;
using Rystem.PlayFramework.Test.Api;
using Rystem.PlayFramework.Test.Api.Services;
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
            var azureApiKey4 = Environment.GetEnvironmentVariable("Azure4ApiKey") ?? configuration["Azure4:ApiKey"];
            var resourceName4 = Environment.GetEnvironmentVariable("Azure4ResourceName") ?? configuration["Azure4:ResourceName"];
            var azureApiKey5 = Environment.GetEnvironmentVariable("Azure5ApiKey") ?? configuration["Azure5:ApiKey"];
            var resourceName5 = Environment.GetEnvironmentVariable("Azure5ResourceName") ?? configuration["Azure5:ResourceName"];
            services.AddLogging(builder => builder.AddConsole());
            services.AddHttpClient("client", x =>
            {
                x.BaseAddress = new Uri("http://localhost");
            });
            services.AddMemoryCache();
            services.AddSingleton<IdentityManager>();
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
                settings.DefaultVersion = "2024-08-01-preview";
                settings.Azure.ResourceName = resourceName;
                settings.Azure.AppRegistration.ClientId = clientId;
                settings.Azure.AppRegistration.ClientSecret = clientSecret;
                settings.Azure.AppRegistration.TenantId = tenantId;
                settings.DefaultRequestConfiguration.Chat = chatClient =>
                {
                    chatClient.ForceModel("gpt-4");
                    chatClient.WithVersion("2024-08-01-preview");
                };
                settings.DefaultRequestConfiguration.Moderation = moderationClient =>
                {
                    moderationClient
                        .WithVersion("2024-10-01-preview");
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
                    settings.DefaultVersion = "2024-02-01";
                    settings.DefaultRequestConfiguration.Speech = audioClient =>
                    {
                        audioClient.ForceModel("2024-05-01-preview");
                    };
                    settings.DefaultRequestConfiguration.Audio = audioClient =>
                    {
                        audioClient.ForceModel("2024-06-01");
                    };
                    settings.DefaultRequestConfiguration.Chat = chatClient =>
                    {
                        chatClient.ForceModel("gpt-4");
                    };
                    settings.PriceBuilder.AddModel("gpt-4",
                        new OpenAiCost { UnitOfMeasure = UnitOfMeasure.Tokens, Kind = KindOfCost.Input, Units = 0.0000015m },
                         new OpenAiCost { UnitOfMeasure = UnitOfMeasure.Tokens, Kind = KindOfCost.Output, Units = 0.0000025m },
                          new OpenAiCost { UnitOfMeasure = UnitOfMeasure.Tokens, Kind = KindOfCost.CachedInput, Units = 0.0000005m });
                }, "Azure2");
            services
               .AddOpenAi(settings =>
               {
                   settings.ApiKey = azureApiKey3;
                   settings.Azure.ResourceName = resourceName3;
                   settings.DefaultVersion = "2024-02-01";
                   settings.DefaultRequestConfiguration.Speech = audioClient =>
                   {
                       audioClient
                        .ForceModel("tts-hd")
                        .WithVersion("2024-05-01-preview");
                   };
                   settings.DefaultRequestConfiguration.Audio = audioClient =>
                   {
                       audioClient
                        .ForceModel("tts-hd")
                        .WithVersion("2024-06-01");
                   };
                   settings.DefaultRequestConfiguration.Chat = chatClient =>
                   {
                       chatClient.ForceModel("gpt-4");
                   };
                   settings.DefaultRequestConfiguration.Chat = chatClient =>
                   {
                       chatClient.ForceModel("gpt-4");
                   };
               }, "Azure3")
               .AddOpenAi(settings =>
               {
                   settings.ApiKey = azureApiKey4;
                   settings.Azure.ResourceName = resourceName4;
                   settings.DefaultRequestConfiguration.Image = imageClient =>
                   {
                       imageClient
                        .ForceModel("dall-e-3-2");
                   };
               }, "Azure4")
               .AddOpenAi(settings =>
               {
                   settings.ApiKey = azureApiKey5;
                   settings.Azure.ResourceName = resourceName5;
                   settings.DefaultRequestConfiguration.Image = imageClient =>
                   {
                       imageClient
                            .ForceModel(ImageModelName.Dalle2);
                   };
               }, "AzureForDalle2")
               .AddOpenAi(x =>
               {
                   x.ApiKey = configuration["Azure6:ApiKey"]!;
                   x.Azure.ResourceName = configuration["Azure6:ResourceName"]!;
                   x.DefaultVersion = "2024-08-01-preview";
                   x.DefaultRequestConfiguration.Chat = chatClient =>
                   {
                       chatClient.ForceModel(configuration["Azure6:ModelName"]!);
                   };
                   x.DefaultRequestConfiguration.RealTime = realTimeClient =>
                   {
                       realTimeClient.WithVersion("2024-10-01-preview");
                       realTimeClient.ForceModel(configuration["Azure6:RealTimeModelName"]!);
                       realTimeClient.WithInputAudioTranscription().WithModel(configuration["Azure6:AudioModelName"]!);
                   };
                   x.PriceBuilder
                   .AddModel(ChatModelName.Gpt_4o,
                   new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                   new OpenAiCost { Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00000125m },
                   new OpenAiCost { Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00001m });
               }, "Azure6")
               .ConfigureOpenAiLogging(x =>
               {
                   x.Request = LogLevel.Information;
                   x.Error = LogLevel.Error;
               });
            services.AddSingleton<WeatherService>();
            services.AddSingleton<VacationService>();
            services
                .AddPlayFramework(scenes =>
            {
                scenes.Configure(settings =>
                {
                    settings.OpenAi.Name = "Azure2";
                    // Enable planning for multi-scene orchestration
                    settings.Planning.Enabled = true;
                    settings.Planning.MaxScenesInPlan = 5;
                    // Enable summarization with lower thresholds for testing
                    settings.Summarization.Enabled = true;
                    settings.Summarization.ResponseThreshold = 20; // Lower for testing
                    settings.Summarization.CharacterThreshold = 3000; // Lower for testing
                })
                .AddCache(cacheBuilder =>
                {
                    cacheBuilder
                        .WithMemory();
                })
                .AddMainActor((context) => $"Oggi è {DateTime.UtcNow:yyyy-MM-dd}. You are a helpful AI assistant.", true)
                .AddScene(scene =>
                {
                    scene
                        .WithName("Weather")
                        .WithDescription("Get information about the weather and manage cities/countries in the database")
                        .WithHttpClient("apiDomain")
                        .WithOpenAi("Azure2")
                        .WithService<WeatherService>(serviceBuilder =>
                        {
                            serviceBuilder
                                .WithMethod(x => x.AddCountryAsync, "Add country", "Used to add country when it does not exist")
                                .WithMethod(x => x.AddCityAsync, "Add city", "Used to add city when it does not exist")
                                .WithMethod(x => x.ExistsAsync, "Check if country exists", "Used to check if the country exists")
                                .WithMethod(x => x.CityExistsAsync, "Check if city exists", "Used to check if the city exists")
                                .WithMethod(x => x.DeleteCityAsync, "Delete city", "Used to delete the city")
                                .WithMethod(x => x.DeleteCountryAsync, "Delete country", "Used to delete the country")
                                .WithMethod(x => x.GetCitiesAsync, "Get cities", "Used to get the cities")
                                .WithMethod(x => x.GetCountriesAsync, "Get country", "Used to get the country")
                                .WithMethod(x => x.ReadCityByIdAsync, "Get city by id", "Used to get the city by id")
                                .WithMethod(x => x.Get, "Get weather", "Get weather forecast for a specific city");
                        })
                            .WithActors(actors =>
                            {
                                actors
                                    .AddActor("If the requested city doesn't exist, add it with population data using the appropriate function.")
                                    .AddActor("Remember to always add the country first if it doesn't exist, using the appropriate function.")
                                    .AddActor("Don't call weather forecast until you're sure everything is properly populated.")
                                    .AddActor<ActorWithDbRequest>();
                            });
                })
                .AddScene(scene =>
                {
                    scene
                    .WithName("Identity")
                    .WithDescription("Get information about the user. Retrieve user details like name based on username.")
                    .WithOpenAi("Azure2")
                    .WithService<IdentityManager>(builder =>
                    {
                        builder.WithMethod(x => x.GetNameAsync, "get_user_name", "Get the full name of a user by their username");
                    });
                })
                .AddScene(scene =>
                {
                    scene
                        .WithName("Chiedi ferie o permessi.")
                        .WithDescription("Manage vacation and leave requests. Submit requests, check approvers, and verify available dates.")
                        .WithOpenAi("Azure2")
                        .WithActors(actorBuilder =>
                        {
                            actorBuilder
                                .AddActor("If the request is unclear, ask for clarification. If dates don't include a year, use the current year.")
                                .AddActor("Always exclude holidays - users know these won't be counted, no need to ask.")
                                .AddActor($"The UserId is {Guid.NewGuid()}");
                        })
                        .WithService<VacationService>(serviceBuilder =>
                        {
                            serviceBuilder
                                .WithMethod(x => x.MakeRequest, "eseguire_richiesta_ferie_permessi", "Submit a vacation or leave request")
                                .WithMethod(x => x.GetApprovers, "prendi_approvatori_richiesta", "Get the list of email addresses who need to approve the request")
                                .WithMethod(x => x.GetAvailableDates, "prendi_date_festive", "Get the list of holiday dates when vacation cannot be requested");
                        });
                });
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
