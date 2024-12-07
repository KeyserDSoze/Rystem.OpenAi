using System.Net.Http.Headers;
using System.Text;

namespace Rystem.PlayFramework
{
    internal sealed class HttpBringer
    {
        public string? RewrittenUri { get; internal set; }
        public string? Method { get; set; }
        public StringBuilder? Query { get; internal set; }
        public HttpRequestHeaders Headers { get; }
        public string? BodyAsJson { get; internal set; }
        public HttpBringer()
        {
            Headers = new HttpRequestMessage().Headers;
            Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
