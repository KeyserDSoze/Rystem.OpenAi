using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Management
{
    public sealed class DeploymentRequest : IOpenAiRequest
    {
        [JsonPropertyName("scale_settings")]
        public DeploymentScaleSettings? ScaleSettings { get; set; }
        [JsonPropertyName("model")]
        public string? ModelId { get; set; }
    }
}
