using Microsoft.Extensions.DependencyInjection;

namespace Rystem.PlayFramework
{
    public interface IServiceBuilder
    {
        IServiceBuilder AddCustomPlanner<TPlanner>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TPlanner : class, IPlanner;
        IServiceBuilder AddCustomSummarizer<TSummarizer>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TSummarizer : class, ISummarizer;
        IServiceBuilder AddCustomResponseParser<TResponseParser>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TResponseParser : class, IResponseParser;
    }
}
