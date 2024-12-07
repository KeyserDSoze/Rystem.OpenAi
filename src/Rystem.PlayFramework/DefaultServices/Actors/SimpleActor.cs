namespace Rystem.PlayFramework
{
    internal sealed class SimpleActor : IActor
    {
        public required string Role { get; init; }
        public Task<ActorResponse> PlayAsync(SceneContext context, CancellationToken cancellationToken) => Task.FromResult(new ActorResponse { Message = Role })!;
    }
}
