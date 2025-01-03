
namespace Rystem.PlayFramework.Test.Api
{
    internal sealed class ActorWithDbRequest : IActor
    {
        public async Task<ActorResponse> PlayAsync(SceneContext context, CancellationToken cancellationToken)
        {
            await Task.Delay(0, cancellationToken);
            return new ActorResponse { Message = string.Empty };
        }
    }
}
