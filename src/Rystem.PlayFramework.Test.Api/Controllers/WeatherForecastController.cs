using Microsoft.AspNetCore.Mvc;

namespace Rystem.PlayFramework.Test.Api
{
    /// <summary>
    /// Controller for managing weather forecasts.
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] s_summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        /// <summary>
        /// Retrieves a list of weather forecasts for a specified city.
        /// </summary>
        /// <remarks>
        /// This endpoint expects a <c>city</c> query parameter.
        /// </remarks>
        /// <param name="city">The city to get weather forecasts for.</param>
        /// <returns>An <see cref="IEnumerable{WeatherForecast}"/> containing the list of weather forecasts.</returns>
        /// <example>
        /// GET /WeatherForecast/Get?city=New%20York
        /// </example>
        [HttpGet]
        public IEnumerable<WeatherForecast> Get([FromQuery] string city)
        {
            _ = city;
            return [.. Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = 20,
                Summary = s_summaries[Random.Shared.Next(s_summaries.Length)]
            })];
        }
    }
}
