using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Client event: response.create.
    /// </summary>
    public class RealTimeClientResponseCreateEvent : RealTimeClientEvent
    {
        [JsonPropertyName("response")]
        public RealTimeClientResponseCreateData Response { get; set; }
    }
}
