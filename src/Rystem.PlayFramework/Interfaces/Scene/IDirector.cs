namespace Rystem.PlayFramework
{
    public interface IDirector
    {
        Task<DirectorResponse> DirectAsync(SceneContext context, SceneRequestSettings requestSettings, CancellationToken cancellationToken);
    }
}
