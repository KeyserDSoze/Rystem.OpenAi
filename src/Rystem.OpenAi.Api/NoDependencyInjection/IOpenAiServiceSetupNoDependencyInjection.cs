using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    public interface IOpenAiServiceSetupNoDependencyInjection
    {
        IOpenAiServiceSetupNoDependencyInjection AddOpenAi(Action<OpenAiSettings> settings, string? integrationName = default);
        ValueTask<List<AutomaticallyDeploymentResult>> MapDeploymentsAutomaticallyAsync(bool forceDeploy = false, params string[] integrationNames);
    }
}
