namespace Rystem.PlayFramework
{
    public interface ICustomCache
    {
        Task<List<AiSceneResponse>> GetAsync(string id, CancellationToken cancellationToken);
        ValueTask<bool> SetAsync(
            string id,
            List<AiSceneResponse> aiSceneResponses,
            CacheSettings? customCacheSettings = null,
            CancellationToken cancellationToken = default);
    }
}
