namespace Rystem.PlayFramework
{
    internal interface ICacheService
    {
        Task<List<AiSceneResponse>> GetAsync(string id, CancellationToken cancellationToken);
        ValueTask<bool> SetAsync(
            string id,
            List<AiSceneResponse> aiSceneResponses,
            Action<CacheSettings>? customCacheSettings = null,
            CancellationToken cancellationToken = default);
    }
}
