using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Rystem.PlayFramework
{
    internal sealed class CacheService : ICacheService
    {
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

            async ValueTask CheckAsync(bool firstTime)
            {
                if (!customCacheAlreadyChecked && _customCache != null)
                {
                    customCacheAlreadyChecked = true;
                    value = await _customCache.GetAsync(id, cancellationToken);
                }
                if (!memoryCacheAlreadyChecked && (value == null && (_cacheSettings.MemoryIsDefault || !firstTime)) && _memoryCache != null)
                {
                    memoryCacheAlreadyChecked = true;
                    _memoryCache.TryGetValue(id, out value);
                }
                if (distributedCacheAlreadyChecked && (value == null && (_cacheSettings.DistributedIsDefault || !firstTime)) && _distributedCache != null)
                {
                    distributedCacheAlreadyChecked = true;
                    await _distributedCache.GetStringAsync(id, cancellationToken).ContinueWith(x =>
                    {
                        if (x.Result != null)
                            value = x.Result.FromJson<List<AiSceneResponse>>();
                    }, cancellationToken);
                }
            }
            return value ?? [];
        }

        public async ValueTask<bool> SetAsync(string id, List<AiSceneResponse> aiSceneResponses, CancellationToken cancellationToken)
        {
            var check = true;
            if (_customCache != null)
                check &= await _customCache.SetAsync(id, aiSceneResponses, cancellationToken);

            if (_memoryCache != null)
            {
                _memoryCache.Set(id, aiSceneResponses);
                check &= true;
            }

            if (_distributedCache != null)
            {
                await _distributedCache.SetStringAsync(id, aiSceneResponses.ToJson(), cancellationToken);
                check &= true;
            }
            return check;
        }
    }
}
