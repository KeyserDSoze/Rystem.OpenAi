using System.Text.Json.Serialization;

namespace Rystem.PlayFramework
{
    public sealed class AiSceneResponse
    {
        [JsonPropertyName("rk")]
        public required string RequestKey { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("functionName")]
        public string? FunctionName { get; set; }
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("args")]
        public object? Arguments { get; set; }
        [JsonPropertyName("response")]
        public string? Response { get; set; }
        [JsonPropertyName("status")]
        public required AiResponseStatus Status { get; set; }
        [JsonPropertyName("responseTime")]
        public required DateTime ResponseTime { get; set; }
    }
}
