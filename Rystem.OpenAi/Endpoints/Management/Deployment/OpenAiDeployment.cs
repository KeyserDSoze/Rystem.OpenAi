using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
            => new DeploymentBuilder(Client, _configuration, Utility, deploymentId, false);
        public DeploymentBuilder Update(string deploymentId)
            => new DeploymentBuilder(Client, _configuration, Utility, deploymentId, true);
        public ValueTask<DeploymentResults> ListAsync(CancellationToken cancellationToken = default)
            => Client.GetAsync<DeploymentResults>(_configuration.GetUri(OpenAiType.Deployment, string.Empty, false, string.Empty), _configuration, cancellationToken);

        public ValueTask<DeploymentResult> RetrieveAsync(string deploymentId, CancellationToken cancellationToken = default)
            => Client.GetAsync<DeploymentResult>(_configuration.GetUri(OpenAiType.Deployment, string.Empty, false, $"/{deploymentId}"), _configuration, cancellationToken);
        public async ValueTask<bool> DeleteAsync(string deploymentId, CancellationToken cancellationToken = default)
        {
            _ = await Client.DeleteAsync<bool>(_configuration.GetUri(OpenAiType.Deployment, string.Empty, false, $"/{deploymentId}"), _configuration, cancellationToken);
            return true;
        }
    }
}
