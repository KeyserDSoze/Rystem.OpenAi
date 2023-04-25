using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rystem.OpenAi;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static async ValueTask<List<AutomaticallyDeploymentResult>> MapDeploymentsAutomaticallyAsync(this IServiceProvider serviceProvider,
            bool forceDeploy = false,
            params string[] integrationNames)
        {
            var events = new List<AutomaticallyDeploymentResult>();
            var services = serviceProvider.CreateScope().ServiceProvider;
            var openAiFactory = services.GetService<IOpenAiFactory>()!;
            var configurations = services.GetService<IEnumerable<OpenAiConfiguration>>()!;
            foreach (var integrationName in integrationNames)
            {
                var openAi = openAiFactory.CreateManagement(integrationName);
                var configuration = configurations.First(x => x.Name == integrationName);
                if (configuration.WithAzure)
                {
                    var availableDeployments = (await openAi.Deployment.ListAsync()).Succeeded;
                    foreach (var deployment in availableDeployments)
                    {
                        configuration.Settings.Azure
                            .MapDeploymentCustomModel(deployment.Id, deployment.ModelId);
                        events.Add(new AutomaticallyDeploymentResult()
                        {
                            IntegrationName = integrationName,
                            Description = $"Mapped {deployment.Id} with {deployment.ModelId}"
                        });
                    }
                    if (forceDeploy)
                        foreach (var deployment in configuration.Settings.Azure.Deployments.Where(x => x.Key != "{0}"))
                        {
                            if (!availableDeployments.Any(x => x.ModelId == deployment.Value))
                            {
                                try
                                {
                                    _ = await openAi.Deployment.Create()
                                        .WithCapacity(1)
                                        .WithScaling(Rystem.OpenAi.Management.DeploymentScaleType.Standard)
                                        .WithDeploymentCustomModel(deployment.Key, deployment.Value)
                                        .ExecuteAsync();
                                    events.Add(new AutomaticallyDeploymentResult()
                                    {
                                        IntegrationName = integrationName,
                                        Description = $"Created {deployment.Key} with {deployment.Value}"
                                    });
                                }
                                catch (Exception exception)
                                {
                                    events.Add(new AutomaticallyDeploymentResult()
                                    {
                                        IntegrationName = integrationName,
                                        Description = $"Failed to create {deployment.Key} with {deployment.Value}",
                                        Exception = exception
                                    });
                                }
                            }
                        }
                    configuration.ConfigureEndpoints();
                }
            }
            return events;
        }
    }
}
