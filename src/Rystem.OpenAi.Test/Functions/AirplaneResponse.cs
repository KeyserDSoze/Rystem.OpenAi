using System;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class AirplaneResponse
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Description { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
