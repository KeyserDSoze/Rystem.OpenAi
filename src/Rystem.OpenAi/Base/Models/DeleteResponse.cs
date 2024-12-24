using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public sealed class DeleteResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }
    }
}
