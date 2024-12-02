using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public enum FineTuneStatus
    {
        [JsonPropertyName("validating_files")]
        ValidatingFiles,
        [JsonPropertyName("queued")]
        Queued,
        [JsonPropertyName("running")]
        Running,
        [JsonPropertyName("succeeded")]
        Succeeded,
        [JsonPropertyName("failed")]
        Failed,
        [JsonPropertyName("cancelled")]
        Cancelled
    }
}
