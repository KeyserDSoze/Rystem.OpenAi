using System;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiLoggerFactory : IOpenAiLoggerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private OpenAiType[]? _types;

        public OpenAiLoggerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IOpenAiLoggerFactory ConfigureTypes(params OpenAiType[] types)
        {
            _types = types;
            return this;
        }
        public IOpenAiLogger Create()
        {
            var logger = _serviceProvider.GetRequiredService<IOpenAiLogger>();
            logger
                .CreateId()
                .ConfigureTypes(_types ?? []);
            return logger;
        }
    }
}
