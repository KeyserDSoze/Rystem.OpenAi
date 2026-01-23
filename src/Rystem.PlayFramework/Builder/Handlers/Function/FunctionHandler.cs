using Rystem.OpenAi.Chat;

namespace Rystem.PlayFramework
{
    internal sealed class FunctionHandler
    {
        public List<string> Scenes { get; } = [];
        public bool ForEveryScene { get; set; }
        public HttpHandler? HttpRequest { get; set; }
        public ServiceHandler? Service { get; set; }
        public McpToolCall? McpToolCall { get; set; }
        public bool HasHttpRequest => HttpRequest != null;
        public bool HasService => Service != null;
        public bool HasMcpToolCall => McpToolCall != null;
        public Action<IOpenAiChat>? Chooser { get; set; }
    }
}
