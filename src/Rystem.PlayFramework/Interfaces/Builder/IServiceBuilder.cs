using Microsoft.Extensions.DependencyInjection;

namespace Rystem.PlayFramework
{
    public interface IServiceBuilder
    {
        IServiceBuilder AddCustomSummarizer<TSummarizer>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TSummarizer : class, ISummarizer;
        IServiceBuilder AddCustomResponseParser<TResponseParser>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TResponseParser : class, IResponseParser;
    }
}
