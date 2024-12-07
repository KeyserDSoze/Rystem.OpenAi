namespace Rystem.PlayFramework
{
    internal sealed class ActionActor : IActor
    {
        public required Func<SceneContext, string> Action { get; init; }
        public Task<ActorResponse> PlayAsync(SceneContext context, CancellationToken cancellationToken) => Task.FromResult(new ActorResponse { Message = Action(context) })!;
    }
}
