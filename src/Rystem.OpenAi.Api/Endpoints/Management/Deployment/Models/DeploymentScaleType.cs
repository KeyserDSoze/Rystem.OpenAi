namespace Rystem.OpenAi.Management
{
    public enum DeploymentScaleType
    {
        /// <summary>
        /// Scaling of a deployment will happen by manually specifying the capacity of a model.
        /// </summary>
        Manual,
        /// <summary>
        /// Scaling of a deployment will happen automatically based on usage.
        /// </summary>
        Standard
    }
}
