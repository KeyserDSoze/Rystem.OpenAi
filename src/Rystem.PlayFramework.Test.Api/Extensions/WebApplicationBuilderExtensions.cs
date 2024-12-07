using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

namespace Rystem.PlayFramework.Test.Api
{
    /// <summary>
    /// Web application builder extensions
    /// </summary>
    public static class WebApplicationBuilderExtensions
    {
        /// <summary>
        /// Use middlewares
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
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
