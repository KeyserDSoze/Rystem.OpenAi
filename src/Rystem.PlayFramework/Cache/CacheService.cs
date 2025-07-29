using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Rystem.PlayFramework
{
    internal sealed class CacheService : ICacheService
    {
        private const int DefaultExpirationInMinutes = 15;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(DefaultExpirationInMinutes);
        private readonly CacheSettings _cacheSettings;
        private readonly IMemoryCache? _memoryCache;
        private readonly IDistributedCache? _distributedCache;
        private readonly ICustomCache? _customCache;

        public CacheService(CacheSettings cacheSettings, IMemoryCache? memoryCache = null, IDistributedCache? distributedCache = null, ICustomCache? customCache = null)
        {
            _cacheSettings = cacheSettings;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _customCache = customCache;
        }

        public async Task<List<AiSceneResponse>> GetAsync(string id, CancellationToken cancellationToken)
        {
            List<AiSceneResponse>? value = null;
            var customCacheAlreadyChecked = false;
            var memoryCacheAlreadyChecked = false;
            var distributedCacheAlreadyChecked = false;
            if (value == null)
            {
                await CheckAsync(true);
                await CheckAsync(false);
            }

            return value ?? [];

            async ValueTask CheckAsync(bool firstTime)
            {
                if (!customCacheAlreadyChecked && _customCache != null)
                {
                    customCacheAlreadyChecked = true;
                    value = await _customCache.GetAsync(id, cancellationToken);
                    if (value != null) return;
                }
            
                if (ShouldCheckMemoryCache(firstTime))
                {
                    memoryCacheAlreadyChecked = true;
                    if (_memoryCache!.TryGetValue(id, out value) && value != null)
                        return;
                }
            
                if (ShouldCheckDistributedCache(firstTime))
                {
                    distributedCacheAlreadyChecked = true;
                    value = await GetFromDistributedCacheAsync();
                }
            }
            
            bool ShouldCheckMemoryCache(bool firstTime) =>
                !memoryCacheAlreadyChecked && 
                value == null && 
                (_cacheSettings.MemoryIsDefault || !firstTime) && 
                _memoryCache != null;
            
            bool ShouldCheckDistributedCache(bool firstTime) =>
                !distributedCacheAlreadyChecked && 
                value == null && 
                (_cacheSettings.DistributedIsDefault || !firstTime) && 
                _distributedCache != null;
            
            async ValueTask<List<AiSceneResponse>?> GetFromDistributedCacheAsync()
            {
                try
                {
                    var cachedData = await _distributedCache!.GetStringAsync(id, cancellationToken);
                    return string.IsNullOrEmpty(cachedData) ? null : cachedData.FromJson<List<AiSceneResponse>>();
                }
                catch
                {
                    // Log dell'errore se necessario
                    return null;
                }
            }
        }

        public async ValueTask<bool> SetAsync(string id, List<AiSceneResponse> aiSceneResponses, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            var check = true;
            if (_customCache != null)
                check &= await _customCache.SetAsync(id, aiSceneResponses, _cacheSettings.ExpirationDefault ?? _defaultExpiration, cancellationToken);

            if (_memoryCache != null)
            {
                _memoryCache.Set(id, aiSceneResponses, _cacheSettings.ExpirationDefault ?? _defaultExpiration);
                check &= true;
            }

            if (_distributedCache != null)
            {
                var distributedCacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheSettings.ExpirationDefault ?? _defaultExpiration
                };
                await _distributedCache.SetStringAsync(id, aiSceneResponses.ToJson(), distributedCacheEntryOptions, cancellationToken);
                check &= true;
            }
            return check;
        }
    }
}
