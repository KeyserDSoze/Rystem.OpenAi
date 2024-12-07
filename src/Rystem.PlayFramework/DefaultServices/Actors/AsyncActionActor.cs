namespace Rystem.PlayFramework
{
    internal sealed class AsyncActionActor : IActor
    {
        public required Func<SceneContext, CancellationToken, Task<string>> Action { get; init; }
        public async Task<ActorResponse> PlayAsync(SceneContext context, CancellationToken cancellationToken)
            => new ActorResponse { Message = await Action(context, cancellationToken) };
    }
}
