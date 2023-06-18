using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class WeatherRequest
    {
        [JsonPropertyName("location")]
        [JsonRequired]
        [Description("The city and state, e.g. San Francisco, CA")]
        public string Location { get; set; }
        [JsonPropertyName("unit")]
        [Description("Unit Measure of temperature. e.g. Celsius or Fahrenheit")]
        public string Unit { get; set; }
    }
    internal enum WeatherTemperatureUnit
    {
        Celsius,
        Fahrenheit
    }
    internal sealed class WeatherRequestModel
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("unit")]
        public WeatherTemperatureUnit Unit { get; set; }
    }
    internal sealed class WeatherResponseModel
    {
        [JsonPropertyName("temperature")]
        public decimal Temperature { get; set; }
        [JsonPropertyName("unit")]
        public WeatherTemperatureUnit Unit { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
    internal sealed class WeatherFunction : IOpenAiChatFunction
    {
        private const string NameLabel = "get_current_weather";
        public string Name => NameLabel;
        private const string DescriptionLabel = "Get the current weather in a given location";
        public string Description => DescriptionLabel;
        public Type Input => typeof(WeatherRequest);
        public async Task<object> WrapAsync(string message)
        {
            var request = System.Text.Json.JsonSerializer.Deserialize<WeatherRequest>(message);
            if (request == null)
                await Task.Delay(0);
            return new WeatherResponseModel
            {
                Description = "Sunny",
                Temperature = 22,
                Unit = WeatherTemperatureUnit.Celsius
            };
        }
    }
}
