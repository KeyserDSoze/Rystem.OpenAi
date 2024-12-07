namespace Rystem.PlayFramework
{
    public interface IPlayableActor
    {
        Task<ActorResponse> PlayAsync(SceneContext context, CancellationToken cancellationToken);
    }
}
