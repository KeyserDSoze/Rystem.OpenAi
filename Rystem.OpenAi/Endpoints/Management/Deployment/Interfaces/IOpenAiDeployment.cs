using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Management
{
    /// <summary>
    /// This methods are available only for Azure integration. A deployment represents a model you can use in your Azure OpenAi environment.
    /// You cannot use that model without a deployment of it. You have choose capacity and scale type of your model.
    /// </summary>
    public interface IOpenAiDeployment
    {
        /// <summary>
        /// Creates a new deployment for the Azure OpenAI resource according to the given specification.
        /// </summary>
        /// <param name="deploymentId"></param>
        /// <returns></returns>
        DeploymentBuilder Create(string? deploymentId = null);
        /// <summary>
        /// Updates the mutable details of the deployment with the given deployment-id.
        /// </summary>
        /// <param name="deploymentId"></param>
        /// <returns></returns>
        DeploymentBuilder Update(string deploymentId);
        /// <summary>
        /// Gets the list of deployments owned by the Azure OpenAI resource.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<DeploymentResults> ListAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets details for a single deployment specified by the given deployment-id.
        /// </summary>
        /// <param name="deploymentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<DeploymentResult> RetrieveAsync(string deploymentId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the deployment specified by the given deployment-id.
        /// </summary>
        /// <param name="deploymentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> DeleteAsync(string deploymentId, CancellationToken cancellationToken = default);
    }
}
