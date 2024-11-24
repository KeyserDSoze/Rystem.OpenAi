using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Management
{
    public sealed class DeploymentScaleSettings
    {
        /// <summary>
        /// The constant reserved capacity of the inference endpoint for this deployment.
        /// </summary>
        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }
        /// <summary>
        /// Defines how scaling operations will be executed.
        /// </summary>
        [JsonPropertyName("scale_type")]
        public string? ScaleType { get; set; }
        public static DeploymentScaleSettings GetDefault()
            => new DeploymentScaleSettings
            {
                Capacity = 1,
                ScaleType = "manual"
            };
    }
}
