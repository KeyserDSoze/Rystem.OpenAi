using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Rystem.PlayFramework;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointBuilderExtensions
    {
        public static IApplicationBuilder MapOpenApiEndpointsForPlayFramework(this IApplicationBuilder app)
        {
            var actorsOpenAiFilter = app.ApplicationServices.GetRequiredService<ActorsOpenAiEndpointParser>();
            actorsOpenAiFilter.MapOpenApiAi(app.ApplicationServices);
            return app;
        }
        public static IApplicationBuilder UseAiEndpoints(this IApplicationBuilder app, params string[] policies)
        {
            return app.UseAiEndpoints(false, policies);
        }
        public static IApplicationBuilder UseAiEndpoints(this IApplicationBuilder app, bool isAuthorized)
        {
            return app.UseAiEndpoints(isAuthorized, []);
        }
        private static IApplicationBuilder UseAiEndpoints(this IApplicationBuilder app, bool authorization, params string[] policies)
        {
            app.UseEndpoints(x =>
            {
                List<RouteHandlerBuilder> routes =
                [
                    x.MapPost("api/ai/message",
                        ([FromQuery(Name = "k")] string? key,
                         [FromQuery(Name = "m")] string message,
                        [FromBody] SceneRequestSettingsForApi settings,
                        [FromServices] ISceneManager sceneManager,
                        CancellationToken cancellationToken) => sceneManager.ExecuteAsync(message, settingBuilder => {
                            settingBuilder.Properties = settings.Properties;
                            settingBuilder.ScenesToAvoid = settings.ScenesToAvoid;
                            if (key != null)
                                settingBuilder.WithKey(key);
                            if (settings.AvoidCache)
                                settingBuilder.AvoidCache();
                        }, cancellationToken)),
                    x.MapGet("api/ai/message",
                       ([FromQuery(Name = "k")] string? key,
                        [FromQuery(Name = "m")] string message,
                       [FromServices] ISceneManager sceneManager,
                       CancellationToken cancellationToken) => sceneManager.ExecuteAsync(message, settingBuilder => {
                           if (key != null)
                                settingBuilder.WithKey(key);
                       }, cancellationToken))
                ];
                foreach (var mapped in routes)
                    if (policies.Length > 0)
                        mapped.RequireAuthorization(policies);
                    else if (authorization)
                        mapped.RequireAuthorization();
            });
            return app;
        }
    }
}
