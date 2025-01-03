using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    /// <summary>
    /// Permissions for using the model
    /// </summary>
    public class Permissions : UnixTimeBaseResponse
    {
        /// <summary>
        /// Permission Id (not to be confused with ModelId)
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        /// <summary>
        /// Object type, should always be 'model_permission'
        /// </summary>
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        /// <summary>
        /// Can the model be created?
        /// </summary>
        [JsonPropertyName("allow_create_engine")]
        public bool AllowCreateEngine { get; set; }
        /// <summary>
        /// Does the model support temperature sampling?
        /// https://beta.openai.com/docs/api-reference/completions/create#completions/create-temperature
        /// </summary>
        [JsonPropertyName("allow_sampling")]
        public bool AllowSampling { get; set; }
        /// <summary>
        /// Does the model support logprobs?
        /// https://beta.openai.com/docs/api-reference/completions/create#completions/create-logprobs
        /// </summary>
        [JsonPropertyName("allow_logprobs")]
        public bool AllowLogProbs { get; set; }
        /// <summary>
        /// Does the model support search indices?
        /// </summary>
        [JsonPropertyName("allow_search_indices")]
        public bool AllowSearchIndices { get; set; }
        [JsonPropertyName("allow_view")]
        public bool AllowView { get; set; }

        /// <summary>
        /// Does the model allow fine tuning?
        /// https://beta.openai.com/docs/api-reference/fine-tunes
        /// </summary>
        [JsonPropertyName("allow_fine_tuning")]
        public bool AllowFineTuning { get; set; }
        /// <summary>
        /// Is the model only allowed for a particular organization? May not be implemented yet.
        /// </summary>
        [JsonPropertyName("organization")]
        public string? Organization { get; set; }
        /// <summary>
        /// Is the model part of a group? Seems not implemented yet. Always null.
        /// </summary>
        [JsonPropertyName("group")]
        public string? Group { get; set; }
        [JsonPropertyName("is_blocking")]
        public bool IsBlocking { get; set; }
    }
}
