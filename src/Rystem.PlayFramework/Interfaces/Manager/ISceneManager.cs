namespace Rystem.PlayFramework
{
    public interface ISceneManager
    {
        IAsyncEnumerable<AiSceneResponse> ExecuteAsync(string message, Action<SceneRequestSettings>? settings = null, CancellationToken cancellationToken = default);
    }
}
