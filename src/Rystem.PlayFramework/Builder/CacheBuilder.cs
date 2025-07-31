using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rystem.PlayFramework
{
    internal sealed class CacheBuilder : ICacheBuilder
    {
        private readonly CacheBehavior _behavior = new();
        private readonly CacheSettings _cacheSettings = new();
        private readonly IServiceCollection _services;

        public CacheBuilder(IServiceCollection services)
        {
            _services = services;
            _services.TryAddSingleton(_behavior);
            _services.TryAddSingleton(_cacheSettings);
            _services.TryAddTransient<ICacheService, CacheService>();
        }
        public ICacheBuilder WithDistributed()
        {
            _behavior.WithDistributed = true;
            return this;
        }

        public ICacheBuilder WithMemory()
        {
            _behavior.WithMemory = true;
            return this;
        }

        public ICacheBuilder WithCustomCache<T>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where T : class, ICustomCache
        {
            _services.TryAddService<ICustomCache>(lifetime);
            _behavior.WithCustom = true;
            return this;
        }

        public ICacheBuilder WithSettings(Action<CacheSettings> customCacheSettings)
        {
            customCacheSettings.Invoke(_cacheSettings);
            return this;
        }
    }
}
