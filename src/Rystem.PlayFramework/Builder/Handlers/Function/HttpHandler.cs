namespace Rystem.PlayFramework
{
    internal sealed class HttpHandler
    {
        public required string Uri { get; init; }
        public Dictionary<string, Func<Dictionary<string, string>, HttpBringer, ValueTask>> Actions { get; } = [];
        public required Func<HttpBringer, ValueTask> Call { get; init; }
    }
}
