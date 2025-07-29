using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rystem.PlayFramework
{
    internal sealed class CacheBuilder : ICacheBuilder
    {
        private readonly CacheSettings _settings = new();
        private readonly IServiceCollection _services;

        public CacheBuilder(IServiceCollection services)
        {
            _services = services;
            _services.TryAddSingleton(_settings);
            _services.TryAddTransient<ICacheService, CacheService>();
        }
        public ICacheBuilder WithDistributed(bool useAsDefault = false)
        {
            _settings.DistributedIsDefault = useAsDefault;
            return this;
        }

        public ICacheBuilder WithMemory(bool useAsDefault = true)
        {
            _settings.MemoryIsDefault = useAsDefault;
            return this;
        }

        public ICacheBuilder WithCustomCache<T>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where T : class, ICustomCache
        {
            _services.TryAddService<ICustomCache>(lifetime);
            return this;
        }
        
        public ICacheBuilder WithCustomExpiration(TimeSpan? expiration = null)
        {
            _settings.ExpirationDefault = expiration;
            return this;
        }
    }
}
