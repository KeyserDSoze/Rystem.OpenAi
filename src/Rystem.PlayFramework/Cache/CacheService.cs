using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Rystem.PlayFramework
{
    internal sealed class CacheService : ICacheService
    {
        private readonly CacheBehavior _cacheBehavior;
        private readonly CacheSettings _cacheSettings;
        private readonly IMemoryCache? _memoryCache;
        private readonly IDistributedCache? _distributedCache;
        private readonly ICustomCache? _customCache;

        public CacheService(CacheBehavior cacheBehavior, CacheSettings cacheSettings, IMemoryCache? memoryCache = null, IDistributedCache? distributedCache = null, ICustomCache? customCache = null)
        {
            _cacheBehavior = cacheBehavior;
            _cacheSettings = cacheSettings;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _customCache = customCache;
        }

        public async Task<List<AiSceneResponse>> GetAsync(string id, CancellationToken cancellationToken)
        {
            // Custom Cache (se attiva)
            if (_cacheBehavior.WithCustom && _customCache != null)
            {
                var customValue = await _customCache.GetAsync(id, cancellationToken);
                if (customValue != null)
                    return customValue;
            }

            if (_cacheBehavior.WithMemory)
            {
                var memoryValue = GetFromMemory(id);
                if (memoryValue != null)
                    return memoryValue;
            }

            if (_cacheBehavior.WithDistributed)
            {
                var distributedValue = await GetFromDistributed(id, cancellationToken);
                if (distributedValue != null)
                    return distributedValue;
            }

            return [];

            List<AiSceneResponse>? GetFromMemory(string key)
            {
                if (_cacheBehavior.WithMemory && _memoryCache != null)
                {
                    if (_memoryCache.TryGetValue(key, out List<AiSceneResponse>? value) && value != null)
                        return value;
                }
                return null;
            }

            async ValueTask<List<AiSceneResponse>?> GetFromDistributed(string key, CancellationToken token)
            {
                if (_cacheBehavior.WithDistributed && _distributedCache != null)
                {
                    try
                    {
                        var cachedData = await _distributedCache.GetStringAsync(key, token);
                        return string.IsNullOrEmpty(cachedData)
                            ? null
                            : cachedData.FromJson<List<AiSceneResponse>>();
                    }
                    catch
                    {
                        // Probably we need a log here, but for now we just return null.
                        return null;
                    }
                }
                return null;
            }
        }

        public async ValueTask<bool> SetAsync(string id, List<AiSceneResponse> aiSceneResponses, Action<CacheSettings>? customCacheSettings = null, CancellationToken cancellationToken = default)
        {
            var cacheSettings = _cacheSettings;
            if (customCacheSettings != null)
            {
                cacheSettings = new CacheSettings();
                customCacheSettings.Invoke(cacheSettings);
            }

            var effectiveExpiration = cacheSettings?.ExpirationDefault;
            var dataToStore = cacheSettings == null ? aiSceneResponses :
                [.. aiSceneResponses.Where(x => cacheSettings.RecordedResponses.HasFlag(x.Status))];

            var check = true;

            if (_cacheBehavior.WithCustom && _customCache != null)
            {
                check &= await _customCache.SetAsync(id, dataToStore, cacheSettings, cancellationToken);
            }

            if (_cacheBehavior.WithMemory && _memoryCache != null)
            {
                if (effectiveExpiration.HasValue)
                    _memoryCache.Set(id, dataToStore, effectiveExpiration.Value);
                else
                    _memoryCache.Set(id, dataToStore);

                check &= true;
            }

            if (_cacheBehavior.WithDistributed && _distributedCache != null)
            {
                if (effectiveExpiration.HasValue)
                {
                    var distributedCacheEntryOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = effectiveExpiration
                    };

                    await _distributedCache.SetStringAsync(id, dataToStore.ToJson(), distributedCacheEntryOptions, cancellationToken);
                }
                else
                    await _distributedCache.SetStringAsync(id, dataToStore.ToJson(), cancellationToken);

                check &= true;
            }

            return check;
        }
    }
}
