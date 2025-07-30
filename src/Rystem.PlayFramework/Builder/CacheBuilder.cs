using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rystem.PlayFramework
{
    internal sealed class CacheBuilder : ICacheBuilder
    {
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(15);
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
        
       public ICacheBuilder WithCustomExpiration(Action<CustomCacheSettings>? customCacheSettings = null)
        {
            if (customCacheSettings == null)
            {
                _settings.ExpirationDefault = _defaultExpiration;
                return this;
            }
            
            var cacheSettings = new CustomCacheSettings();
            customCacheSettings.Invoke(cacheSettings);
            _settings.ExpirationDefault = cacheSettings.ExpirationDefault;
            return this;
        }
    }
}
