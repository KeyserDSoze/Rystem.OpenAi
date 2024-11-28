using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneResult
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        [JsonPropertyName("created_at")]
        public int? CreatedAt { get; set; }
        [JsonPropertyName("events")]
        public List<FineTuneEvent>? Events { get; set; }
        [JsonPropertyName("fine_tuned_model")]
        public string? FineTuneModel { get; set; }
        [JsonPropertyName("hyperparams")]
        public FineTuneHyperParameters? Hyperparams { get; set; }
        [JsonPropertyName("organization_id")]
        public string? OrganizationId { get; set; }
        [JsonPropertyName("status")]
        public FineTuneStatus? Status { get; set; }
        [JsonPropertyName("result_files")]
        public List<string>? ResultFiles { get; set; }
        [JsonPropertyName("validation_file")]
        public string? ValidationFile { get; set; }
        [JsonPropertyName("training_file")]
        public string? TrainingFile { get; set; }
        [JsonPropertyName("updated_at")]
        public int? UpdatedAt { get; set; }
        [JsonPropertyName("finished_at")]
        public int? FinishedAt { get; set; }
        [JsonPropertyName("estimated_finish")]
        public int? EstimatedFinish { get; set; }
        [JsonPropertyName("trained_tokens")]
        public int? TrainedTokens { get; set; }
        [JsonPropertyName("integrations")]
        public List<FineTuneIntegration>? Integrations { get; set; }
        [JsonPropertyName("error")]
        public FineTuneError? Error { get; set; }
    }
}
