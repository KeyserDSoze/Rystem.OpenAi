using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Management
{
    public sealed class DeploymentResult : StatedResult
    {
        [JsonPropertyName("scale_settings")]
        public DeploymentScaleSettings? ScaleSettings { get; set; }
        [JsonPropertyName("owner")]
        public string? Owner { get; set; }
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("created_at")]
        public int CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public int UpdatedAt { get; set; }
    }
}
