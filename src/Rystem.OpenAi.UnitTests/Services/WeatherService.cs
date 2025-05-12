using Microsoft.AspNetCore.Mvc;
using Rystem.PlayFramework;
using Rystem.PlayFramework.Test.Api;

namespace Rystem.OpenAi.UnitTests.Services
{
    internal sealed class ActorWithDbRequest : IActor
    {
        public async Task<ActorResponse> PlayAsync(SceneContext context, CancellationToken cancellationToken)
        {
            await Task.Delay(0, cancellationToken);
            return new ActorResponse { Message = string.Empty };
        }
    }
    internal sealed class WeatherService
    {
        private static readonly string[] s_summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];
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
        /// <summary>
        /// Retrieves a city by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id?}")]
        public async Task<string> ReadCityByIdAsync([FromRoute] string id)
        {
            await Task.Delay(0);
            return id;
        }
        /// <summary>
        /// Adds a new city to the database.
        /// </summary>
        /// <remarks>
        /// This endpoint expects a <see cref="City"/> object in the request body.
        /// </remarks>
        /// <param name="city">The city to add.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        /// <example>
        /// POST /Country/AddCityAsync
        /// Content-Type: application/json
        /// {
        ///   "Name": "New York",
        ///   "Country": "USA",
        ///   "Population": 8419000
        /// }
        /// </example>
        [HttpPost]
        public async Task<bool> AddCityAsync([FromBody] City city)
        {
            await Task.Delay(0);
            _ = city;
            return true;
        }

        /// <summary>
        /// Adds a new country to the database.
        /// </summary>
        /// <remarks>
        /// This endpoint expects a <see cref="Country"/> object in the request body.
        /// </remarks>
        /// <param name="country">The country to add.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        /// <example>
        /// POST /Country/AddCountryAsync
        /// Content-Type: application/json
        /// {
        ///   "Name": "USA",
        ///   "Population": 331000000
        /// }
        /// </example>
        [HttpPost]
        public async Task<bool> AddCountryAsync([FromBody] Country country)
        {
            await Task.Delay(0);
            _ = country;
            return true;
        }

        /// <summary>
        /// Retrieves a list of cities for a specified country.
        /// </summary>
        /// <remarks>
        /// This endpoint expects a <c>country</c> query parameter.
        /// </remarks>
        /// <param name="country">The country to filter cities by.</param>
        /// <returns>An <see cref="IAsyncEnumerable{City}"/> containing the list of cities.</returns>
        /// <example>
        /// GET /Country/GetCities?country=USA
        /// </example>
        [HttpGet]
        public async IAsyncEnumerable<City> GetCitiesAsync([FromQuery] string country)
        {
            await Task.Delay(0);
            yield return new City { Country = country, Name = "City1", Population = 1000000 };
        }

        /// <summary>
        /// Check if a country exists.
        /// </summary>
        /// <remarks>
        /// This endpoint expects a <c>country</c> query parameter.
        /// </remarks>
        /// <param name="country">The country to check if exists in database.</param>
        /// <returns>A <see cref="bool"/> if exists.</returns>
        /// <example>
        /// GET /Country/Exists?country=USA
        /// </example>
        [HttpGet]
        public async Task<bool> ExistsAsync([FromQuery] string country)
        {
            await Task.Delay(10);
            var x = _exist;
            _exist = !_exist;
            _ = country;
            return x;
        }
        private bool _exist = false;
        /// <summary>
        /// Check if a city exists.
        /// </summary>
        /// <remarks>
        /// This endpoint expects a <c>city</c> query parameter.
        /// </remarks>
        /// <param name="city">The country to check if exists in database.</param>
        /// <returns>A <see cref="bool"/> if exists.</returns>
        /// <example>
        /// GET /Country/CityExists?city=New York
        /// </example>
        [HttpGet]
        public async Task<bool> CityExistsAsync([FromQuery] string city)
        {
            await Task.Delay(10);
            var x = _cityAlreadyExists;
            _cityAlreadyExists = !_cityAlreadyExists;
            _ = city;
            return x;
        }
        private bool _cityAlreadyExists;

        /// <summary>
        /// Retrieves a list of all countries.
        /// </summary>
        /// <remarks>
        /// This endpoint does not require any parameters.
        /// </remarks>
        /// <returns>An <see cref="IAsyncEnumerable{Country}"/> containing the list of countries.</returns>
        /// <example>
        /// GET /Country/GetCountriesAsync
        /// </example>
        [HttpGet]
        public async IAsyncEnumerable<Country> GetCountriesAsync()
        {
            await Task.Delay(0);
            yield return new Country { Name = "", Population = 32 };
        }

        /// <summary>
        /// Deletes a city by its name.
        /// </summary>
        /// <remarks>
        /// This endpoint expects a <c>name</c> query parameter.
        /// </remarks>
        /// <param name="name">The name of the city to delete.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        /// <example>
        /// DELETE /Country/DeleteCityAsync?name=New%20York
        /// </example>
        [HttpDelete]
        public async Task<bool> DeleteCityAsync(string name)
        {
            await Task.Delay(0);
            _ = name;
            return true;
        }

        /// <summary>
        /// Deletes a country by its name.
        /// </summary>
        /// <remarks>
        /// This endpoint expects a <c>name</c> query parameter.
        /// </remarks>
        /// <param name="name">The name of the country to delete.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        /// <example>
        /// DELETE /Country/DeleteCountryAsync?name=USA
        /// </example>
        [HttpDelete]
        public async Task<bool> DeleteCountryAsync(string name)
        {
            await Task.Delay(0);
            _ = name;
            return true;
        }
    }
}
