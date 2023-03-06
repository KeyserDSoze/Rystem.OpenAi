using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneEventsResult : ApiBaseResponse
    {
        [JsonPropertyName("data")]
        public List<FineTuneEvent>? Data { get; set; }
    }

}
