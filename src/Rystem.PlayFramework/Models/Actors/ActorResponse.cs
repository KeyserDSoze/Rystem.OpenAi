namespace Rystem.PlayFramework
{
    public sealed class ActorResponse
    {
        public string? Message { get; set; }
        public static ActorResponse Empty => new();
    }
}
