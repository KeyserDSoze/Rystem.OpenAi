using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public class FineTuningDeleteResult : OpenAiObjectWithId
    {
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }
    }
}
