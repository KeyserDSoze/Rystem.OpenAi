
namespace Rystem.PlayFramework.Test.Api
{
    public sealed class City
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public int Population { get; set; }
    }
    public sealed class Country
    {
        public string Name { get; set; }
        public int Population { get; set; }
    }
    internal sealed class ActorWithDbRequest : IActor
    {
        public async Task<ActorResponse> PlayAsync(SceneContext context, CancellationToken cancellationToken)
        {
            await Task.Delay(0);
            return new ActorResponse { Message = string.Empty };
        }
    }
}
