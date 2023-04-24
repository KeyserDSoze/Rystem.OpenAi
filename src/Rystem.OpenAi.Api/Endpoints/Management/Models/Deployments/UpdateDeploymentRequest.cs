using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Management
{
    public sealed class UpdateDeploymentRequest
    {
        [JsonPropertyName("scale_settings")]
        public DeploymentScaleSettings ScaleSettings { get; set; }
    }
}
