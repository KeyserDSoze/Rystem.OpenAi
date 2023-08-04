using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    public interface IOpenAiServiceSetupNoDependencyInjection
    {
        IServiceCollection Services { get; }
        IOpenAiServiceSetupNoDependencyInjection AddOpenAi(Action<OpenAiSettings> settings, string? integrationName = default);
        IOpenAiServiceSetupNoDependencyInjection AddFurtherService<TService, TImplementation>(ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService;
        ValueTask<List<AutomaticallyDeploymentResult>> MapDeploymentsAutomaticallyAsync(bool forceDeploy = false, params string[] integrationNames);
    }
}
