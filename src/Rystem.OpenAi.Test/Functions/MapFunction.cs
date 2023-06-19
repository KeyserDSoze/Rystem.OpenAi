using System;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Test.Functions
{
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
