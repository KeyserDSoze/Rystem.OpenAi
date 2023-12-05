using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public abstract class OpenAiObjectWithId
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("object")]
        public string? Object { get; set; }
    }
}
