using Microsoft.Extensions.DependencyInjection;

namespace Rystem.PlayFramework
{
    public interface ICacheBuilder
    {
        ICacheBuilder WithMemory();
        ICacheBuilder WithDistributed();
        ICacheBuilder WithCustomCache<T>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, ICustomCache;
        ICacheBuilder WithSettings(Action<CacheSettings> cacheSettings);
    }
}
