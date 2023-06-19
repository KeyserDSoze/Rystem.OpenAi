using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class WeatherFunction : IOpenAiChatFunction
    {
        private static readonly JsonSerializerOptions s_options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };
        static WeatherFunction()
        {
            var converter = new JsonStringEnumConverter();
            s_options.Converters.Add(converter);
        }
        public const string NameLabel = "get_current_weather";
        public string Name => NameLabel;
        private const string DescriptionLabel = "Get the current weather in a given location";
        public string Description => DescriptionLabel;
        public Type Input => typeof(WeatherRequest);
        public async Task<object> WrapAsync(string message)
        {
            var request = System.Text.Json.JsonSerializer.Deserialize<WeatherRequest>(message, s_options);
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
}
