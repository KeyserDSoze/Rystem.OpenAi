using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Embedding
{
    /// <summary>
    /// Represents an embedding result returned by the Embedding API.  
    /// </summary>
    public sealed class EmbeddingResult : ApiBaseResponse
    {
        /// <summary>
        /// List of results of the embedding
        /// </summary>
        [JsonPropertyName("data")]
        public List<EmbeddingData>? Data { get; set; }
        /// <summary>
        /// Usage statistics of how many tokens have been used for this request
        /// </summary>
        [JsonPropertyName("usage")]
        public EmbeddingUsage? Usage { get; set; }
    }

}
