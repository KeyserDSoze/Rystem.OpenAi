using Microsoft.Extensions.DependencyInjection;

namespace Rystem.PlayFramework
{
    [Flags]
    internal enum ServiceBuilderType
    {
        None = 0,
        ResponseParser = 2,
        Summarizer = 4
    }
    internal sealed class ServiceBuilder : IServiceBuilder
    {
        private readonly IServiceCollection _services;
        private ServiceBuilderType _serviceBuilderType;
        public ServiceBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public IServiceBuilder AddCustomResponseParser<TResponseParser>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TResponseParser : class, IResponseParser
        {
            _services.AddService<IResponseParser, TResponseParser>(lifetime);
            _serviceBuilderType |= ServiceBuilderType.ResponseParser;
            return this;
        }

        public IServiceBuilder AddCustomSummarizer<TSummarizer>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TSummarizer : class, ISummarizer
        {
            _services.AddService<ISummarizer, TSummarizer>(lifetime);
            _serviceBuilderType |= ServiceBuilderType.Summarizer;
            return this;
        }
        internal void AddDefaultsIfNeeded()
        {
            if (!_serviceBuilderType.HasFlag(ServiceBuilderType.ResponseParser))
                _services.AddService<IResponseParser, DefaultResponseParser>(ServiceLifetime.Singleton);
            if (!_serviceBuilderType.HasFlag(ServiceBuilderType.Summarizer))
                _services.AddService<ISummarizer, DefaultSummarizer>(ServiceLifetime.Singleton);
        }
    }
}
