using System.Text.Json.Serialization;

namespace Rystem.PlayFramework
{
    public sealed class AiSceneResponse
    {
        [JsonPropertyName("rk")]
        public required string RequestKey { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonPropertyName("nm")]
        public string? Name { get; set; }
        [JsonPropertyName("fn")]
        public string? FunctionName { get; set; }
        [JsonPropertyName("msg")]
        public string? Message { get; set; }
        [JsonPropertyName("args")]
        public string? Arguments { get; set; }
        [JsonPropertyName("rsp")]
        public string? Response { get; set; }
        [JsonPropertyName("s")]
        public required AiResponseStatus Status { get; set; }
        [JsonPropertyName("rt")]
        public required DateTime ResponseTime { get; set; }
    }
}
