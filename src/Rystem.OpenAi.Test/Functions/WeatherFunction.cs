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
    internal enum PositionMap
    {
        TwoDimension,
        ThreeDimension
    }
    internal sealed class CarRequestModel
    {
        [JsonPropertyName("plate")]
        [Description("The plate of the car.")]
        [JsonRequired]
        public string Plate { get; set; }
        [JsonPropertyName("dimension")]
        [Description("Searching with position in 2 dimensions or 3 dimensions.")]
        public PositionMap Dimension { get; set; }
    }
    internal sealed class WeatherResponseModel
    {
        [JsonPropertyName("temperature")]
        public decimal Temperature { get; set; }
        [JsonPropertyName("unit")]
        public string Unit { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
    internal sealed class CarResponseModel
    {
        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }
        [JsonPropertyName("plate")]
        public string Car { get; set; }
    }
    internal sealed class WeatherFunction : IOpenAiChatFunction
    {
        public const string NameLabel = "get_current_weather";
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
                Unit = "Celsius"
            };
        }
    }
    internal sealed class MapFunction : IOpenAiChatFunction
    {
        public const string NameLabel = "get_current_position";
        public string Name => NameLabel;
        private const string DescriptionLabel = "Get the current position of a car";
        public string Description => DescriptionLabel;
        public Type Input => typeof(CarRequestModel);
        public async Task<object> WrapAsync(string message)
        {
            var request = System.Text.Json.JsonSerializer.Deserialize<CarRequestModel>(message);
            if (request == null)
                await Task.Delay(0);
            return new CarResponseModel
            {
                Car = request.Plate,
                Latitude = 23,
                Longitude = 23
            };
        }
    }
}
