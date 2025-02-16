using System.Text.RegularExpressions;
using Rystem.OpenAi;
using Rystem.PlayFramework.Test.Api.Services;

namespace Rystem.PlayFramework.Test.Api
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddOpenAi(x =>
            {
                x.ApiKey = configuration["OpenAi2:ApiKey"]!;
            }, "openai");
            services.AddOpenAi(x =>
            {
                x.ApiKey = configuration["OpenAi:ApiKey"]!;
                x.Azure.ResourceName = configuration["OpenAi:ResourceName"]!;
                x.DefaultVersion = "2024-08-01-preview";
                x.DefaultRequestConfiguration.Chat = chatClient =>
                {
                    chatClient.ForceModel(configuration["OpenAi:ModelName"]!);
                };
                x.DefaultRequestConfiguration.RealTime = realTimeClient =>
                {
                    realTimeClient.WithVersion("2024-12-17");
                    realTimeClient.ForceModel(configuration["OpenAi:RealTimeModelName"]!);
                    realTimeClient.WithInputAudioTranscription().WithModel(configuration["OpenAi:AudioModelName"]!);
                };
                x.PriceBuilder
                .AddModel(ChatModelName.Gpt_4o,
                new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00000125m },
                new OpenAiCost { Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00001m });
            }, "playframework");
            services.AddHttpClient("apiDomain", x =>
            {
                x.BaseAddress = new Uri(configuration["Api:Uri"]!);
            });
            services.AddSingleton<IdentityManager>();
            services.AddPlayFramework(scenes =>
            {
                scenes.Configure(settings =>
                {
                    settings.OpenAi.Name = "playframework";
                })
                .AddMainActor((context) => $"Oggi è {DateTime.UtcNow}.", true)
                .AddScene(scene =>
                {
                    scene
                        .WithName("Weather")
                        .WithDescription("Get information about the weather")
                        .WithHttpClient("apiDomain")
                        .WithOpenAi("playframework")
                        .WithApi(pathBuilder =>
                        {
                            pathBuilder
                                .Map(new Regex("Country/*"))
                                .Map(new Regex("City/*"))
                                .Map("Weather/");
                        })
                            .WithActors(actors =>
                            {
                                actors
                                    .AddActor("Nel caso non esistesse la città richiesta potresti aggiungerla con il numero dei suoi abitanti.")
                                    .AddActor("Ricordati che va sempre aggiunta anche la nazione, quindi se non c'è la nazione aggiungi anche quella.")
                                    .AddActor("Non chiamare alcun meteo prima di assicurarti che tutto sia stato popolato correttamente.")
                                    .AddActor<ActorWithDbRequest>();
                            });
                })
                .AddScene(scene =>
                {
                    scene
                    .WithName("Identity")
                    .WithDescription("Get information about the user")
                    .WithOpenAi("openai")
                    .WithService<IdentityManager>(builder =>
                    {
                        builder.WithMethod(x => x.GetNameAsync);
                    });
                });
            });
            return services;
        }
    }
}
