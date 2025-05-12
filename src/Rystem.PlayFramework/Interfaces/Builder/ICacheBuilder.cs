using Microsoft.Extensions.DependencyInjection;

namespace Rystem.PlayFramework
{
    public interface ICacheBuilder
    {
        ICacheBuilder WithMemory(bool useAsDefault = true);
        ICacheBuilder WithDistributed(bool useAsDefault = false);
        ICacheBuilder WithCustomCache<T>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, ICustomCache;
    }
}
