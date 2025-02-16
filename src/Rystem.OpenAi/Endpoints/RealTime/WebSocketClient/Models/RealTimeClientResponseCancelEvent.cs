using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Client event: response.cancel.
    /// </summary>
    public class RealTimeClientResponseCancelEvent : RealTimeClientEvent
    {
        [JsonPropertyName("response_id")]
        public string ResponseId { get; set; }
    }
}
