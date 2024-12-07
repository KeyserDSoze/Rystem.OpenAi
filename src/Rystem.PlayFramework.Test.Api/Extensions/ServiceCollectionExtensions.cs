using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Rystem.PlayFramework.Test.Api.Services;
using Scalar.AspNetCore;

namespace Rystem.PlayFramework.Test.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddOpenApi();
            services.AddChat(x =>
            {
                x.AddConfiguration("openai", builder =>
                {
                    builder.ApiKey = configuration["OpenAi:ApiKey"]!;
                    builder.Uri = configuration["OpenAi:Endpoint"]!;
                    builder.Model = configuration["OpenAi:ModelName"]!;
                    builder.ChatClientBuilder = (chatClient) =>
                    {
                        chatClient.AddPriceModel(new ChatPriceSettings
                        {
                            InputToken = 0.02M,
                            OutputToken = 0.02M
                        });
                    };
                });
            });
            services.AddHttpClient("apiDomain", x =>
            {
                x.BaseAddress = new Uri(configuration["Api:Uri"]!);
            });
            services.AddSingleton<IdentityManager>();
            services.AddPlayFramework(scenes =>
            {
                scenes.Configure(settings =>
                {
                    settings.OpenAi.Name = "openai";
                })
                .AddMainActor((context) => $"Oggi è {DateTime.UtcNow}.", true)
                .AddScene(scene =>
                {
                    scene
                        .WithName("Weather")
                        .WithDescription("Get information about the weather")
                        .WithHttpClient("apiDomain")
                        .WithOpenAi("openai")
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
    public static class WebApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(x =>
            {
                x.MapOpenApi();
                x.MapScalarApiReference();
            });
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseEndpoints(x =>
            {
                x.MapPost("api/bestwestern", async ([FromQuery] string file) =>
                {
                    return await Task.FromResult("ok");
                });
                x.MapControllers();
            });
            app.MapOpenApiEndpointsForPlayFramework();
            app.UseAiEndpoints();
            return app;
        }
    }
}
