using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rystem.OpenAi;
using Rystem.OpenAi.Management;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// It is a extensions method for IServiceProvider,
        /// with true you can automatically install on Azure the deployments you setup on application.
        /// In the other parameter (integrationNames) you can choose which integration runs this automatic update.
        /// </summary>
        /// <param name="serviceProvider">built services</param>
        /// <param name="forceDeploy">Deploy the not deployed models on Azure.</param>
        /// <param name="integrationNames">Name of integration that you need to check.</param>
        /// <returns></returns>
        public static async ValueTask<List<AutomaticallyDeploymentResult>> MapDeploymentsAutomaticallyAsync(this IServiceProvider serviceProvider,
            bool forceDeploy = false,
            params string[] integrationNames)
        {
            if (integrationNames.Length == 0)
                integrationNames = new string[1] { string.Empty };
            var events = new List<AutomaticallyDeploymentResult>();
            var services = serviceProvider.CreateScope().ServiceProvider;
            var openAiFactory = services.GetService<IOpenAiFactory>()!;
            var configurations = services.GetService<IEnumerable<OpenAiConfiguration>>()!;
            foreach (var integrationName in integrationNames)
            {
                await ConfigureIntegrationAsync(integrationName, openAiFactory, configurations, events, forceDeploy);
            }
            return events;
        }
        private static async ValueTask ConfigureIntegrationAsync(string integrationName, IOpenAiFactory openAiFactory, IEnumerable<OpenAiConfiguration> configurations, List<AutomaticallyDeploymentResult> events, bool forceDeploy)
        {
            var openAi = openAiFactory.CreateManagement(integrationName);
            var configuration = configurations.First(x => x.Name == integrationName);
            if (configuration.WithAzure)
            {
                try
                {
                    var availableDeployments = (await openAi.Deployment.ListAsync()).Succeeded;
                    foreach (var deployment in availableDeployments)
                    {
                        configuration.Settings.Azure
                            .MapDeploymentCustomModel(deployment.Id!, deployment.ModelId!);
                        events.Add(new AutomaticallyDeploymentResult()
                        {
                            IntegrationName = integrationName,
                            Description = $"Mapped {deployment.Id} with {deployment.ModelId}"
                        });
                    }
                    if (forceDeploy)
                        await ForceDeployAsync(integrationName, availableDeployments, configuration, openAi, events);

                    configuration.ConfigureEndpoints();
                }
                catch { }
            }
        }
        private static async ValueTask ForceDeployAsync(string integrationName,
            IEnumerable<DeploymentResult> availableDeployments,
            OpenAiConfiguration configuration,
            IOpenAiManagement openAi,
            List<AutomaticallyDeploymentResult> events)
        {
            foreach (var deployment in configuration.Settings.Azure.Deployments.Where(x => x.Key != "{0}"))
            {
                if (!availableDeployments.Any(x => x.ModelId == deployment.Value))
                {
                    try
                    {
                        _ = await openAi.Deployment.Create()
                            .WithCapacity(1)
                            .WithScaling(DeploymentScaleType.Standard)
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
        }
    }
}
