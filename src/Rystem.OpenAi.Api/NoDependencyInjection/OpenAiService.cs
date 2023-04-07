using System;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    public static class OpenAiService
    {
        private static readonly IServiceCollection s_services = new ServiceCollection();
        private static IServiceProvider? s_serviceProvider;
        public static void Setup(Action<OpenAiSettings> settings, string? name = default)
        {
            s_services.AddOpenAi(settings, name);
        }
        public static IOpenAiApi Create(string? name = default)
        {
            if (s_serviceProvider == null)
                s_serviceProvider = s_services.BuildServiceProvider();
            var factory = s_serviceProvider.GetService<IOpenAiFactory>()!;
            if (name == default)
                name = string.Empty;
            return factory.Create(name);
        }
        public static IOpenAiUtility Utility()
        {
            if (s_serviceProvider == null)
                s_serviceProvider = s_services.BuildServiceProvider();
            return s_serviceProvider.GetService<IOpenAiUtility>()!;
        }
    }
}
