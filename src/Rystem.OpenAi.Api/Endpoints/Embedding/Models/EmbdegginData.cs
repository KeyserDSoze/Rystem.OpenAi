using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Embedding
{
    /// <summary>
    /// Data returned from the Embedding API.
    /// </summary>
    public class EmbeddingData
    {
        /// <summary>
        /// Type of the response. In case of Data, this will be "embedding"  
        /// </summary>
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        /// <summary>
        /// The input text represented as a vector (list) of floating point numbers
        /// </summary>
        [JsonPropertyName("embedding")]
        public float[]? Embedding { get; set; }
        /// <summary>
        /// Index
        /// </summary>
        [JsonPropertyName("index")]
        public int Index { get; set; }

    }

}
