using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneResult : StatedResult
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("created_at")]
        public int CreatedAt { get; set; }
        [JsonPropertyName("error")]
        public object Error { get; set; }
        [JsonPropertyName("events")]
        public List<FineTuneEvent>? Events { get; set; }
        [JsonPropertyName("fine_tuned_model")]
        public object? FineTuneModel { get; set; }
        [JsonPropertyName("hyperparams")]
        public Hyperparams? Hyperparams { get; set; }
        [JsonPropertyName("organization_id")]
        public string? OrganizationId { get; set; }
        [JsonPropertyName("result_files")]
        public List<object>? ResultFiles { get; set; }
        [JsonPropertyName("validation_files")]
        public List<object>? ValidationFiles { get; set; }
        [JsonPropertyName("training_files")]
        public List<TrainingFiles>? TrainingFiles { get; set; }
        [JsonPropertyName("updated_at")]
        public int UpdatedAt { get; set; }
        [JsonPropertyName("trained_tokens")]
        public int TrainedTokens { get; set; }
    }
}
