using System;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class AirplaneFunction : IOpenAiChatFunction
    {
        public const string NameLabel = "get_airplanes";
        public string Name => NameLabel;

        public string Description => "Get the available airplanes for your travel.";
        public Type Input => typeof(AirplaneRequest);

        public async Task<object> WrapAsync(string message)
        {
            var request = System.Text.Json.JsonSerializer.Deserialize<AirplaneRequest>(message);
            if (request == null)
                await Task.Delay(0);
            return new AirplaneResponse
            {
                DepartureTime = DateTime.UtcNow.AddDays(34).AddHours(-6),
                ArrivalTime = DateTime.UtcNow.AddDays(34),
                Description = "On this airplane you have all comfort like the chance to eat a burrito and drink a golden beer.",
                From = request.Position.From,
                To = request.Position.To,
            };
        }
    }
}
