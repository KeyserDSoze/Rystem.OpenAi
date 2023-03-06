using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneResult : ApiBaseResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("created_at")]
        public int CreatedAt { get; set; }
        [JsonPropertyName("events")]
        public FineTuneEvent[]? Events { get; set; }
        [JsonPropertyName("fine_tuned_model")]
        public object? FineTuneModel { get; set; }
        [JsonPropertyName("hyperparams")]
        public Hyperparams? Hyperparams { get; set; }
        [JsonPropertyName("organization_id")]
        public string? OrganizationId { get; set; }
        [JsonPropertyName("result_files")]
        public object[]? ResultFiles { get; set; }
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        [JsonPropertyName("validation_files")]
        public object[]? ValidationFiles { get; set; }
        [JsonPropertyName("training_files")]
        public TrainingFiles[]? TrainingFiles { get; set; }
        [JsonPropertyName("updated_at")]
        public int UpdatedAt { get; set; }
    }
}
