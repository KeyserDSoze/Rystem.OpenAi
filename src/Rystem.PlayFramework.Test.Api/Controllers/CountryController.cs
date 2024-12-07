using Microsoft.AspNetCore.Mvc;

namespace Rystem.PlayFramework.Test.Api
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CountryController : ControllerBase
    {
        private readonly ILogger<CountryController> _logger;

        public CountryController(ILogger<CountryController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        [Route("{id?}")]
        public async Task<IActionResult> ReadCityByIdAsync([FromRoute] string id)
        {
            return Ok(id);
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
        public async Task<IActionResult> AddCityAsync([FromBody] City city)
        {
            return Ok("true");
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
        public async Task<IActionResult> AddCountryAsync([FromBody] Country country)
        {
            return Ok("true");
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
            var x = _cityexist;
            _cityexist = !_cityexist;
            return x;
        }
        private bool _cityexist = false;

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
        public async Task<IActionResult> DeleteCityAsync(string name)
        {
            return Ok("true");
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
        public async Task<IActionResult> DeleteCountryAsync(string name)
        {
            return Ok("true");
        }
    }
}
