using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Management
{
    internal sealed class OpenAiDeployment : OpenAiBase, IOpenAiDeployment
    {
        public OpenAiDeployment(IHttpClientFactory httpClientFactory,
           IEnumerable<OpenAiConfiguration> configurations,
           IOpenAiUtility utility)
           : base(httpClientFactory, configurations, utility)
        {
        }
        public DeploymentBuilder Create(string? deploymentId = null)
            => new DeploymentBuilder(Client, Configuration, Utility, deploymentId, false);
        public DeploymentBuilder Update(string deploymentId)
            => new DeploymentBuilder(Client, Configuration, Utility, deploymentId, true);
        public ValueTask<DeploymentResults> ListAsync(CancellationToken cancellationToken = default)
            => Client.GetAsync<DeploymentResults>(Configuration.GetUri(OpenAiType.Deployment, string.Empty, false, string.Empty), Configuration, cancellationToken);

        public ValueTask<DeploymentResult> RetrieveAsync(string deploymentId, CancellationToken cancellationToken = default)
            => Client.GetAsync<DeploymentResult>(Configuration.GetUri(OpenAiType.Deployment, string.Empty, false, $"/{deploymentId}"), Configuration, cancellationToken);
        public async ValueTask<bool> DeleteAsync(string deploymentId, CancellationToken cancellationToken = default)
        {
            _ = await Client.DeleteAsync<bool>(Configuration.GetUri(OpenAiType.Deployment, string.Empty, false, $"/{deploymentId}"), Configuration, cancellationToken);
            return true;
        }
    }
}
