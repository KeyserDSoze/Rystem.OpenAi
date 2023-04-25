using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Management
{
#warning add State for everything that has State
    public sealed class DeploymentRequest : IOpenAiRequest
    {
        [JsonPropertyName("scale_settings")]
        public DeploymentScaleSettings ScaleSettings { get; set; }
        [JsonPropertyName("model")]
        public string ModelId { get; set; }
    }
}
