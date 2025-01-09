using System;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    public interface IOpenAiServiceLocatorConfigurator
    {
        /// <summary>
        /// Adds OpenAI services with the specified settings and integration name.
        /// </summary>
        /// <param name="settings">Configuration settings for OpenAI.</param>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>The current service locator instance.</returns>
        IOpenAiServiceLocatorConfigurator AddOpenAi(Action<OpenAiSettings> settings, AnyOf<string?, Enum>? integrationName = default);

        /// <summary>
        /// Adds a custom service implementation to the service collection.
        /// </summary>
        /// <typeparam name="TService">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The service implementation type.</typeparam>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <returns>The current service locator instance.</returns>
        IOpenAiServiceLocatorConfigurator AddFurtherService<TService, TImplementation>(ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService;
    }
}
