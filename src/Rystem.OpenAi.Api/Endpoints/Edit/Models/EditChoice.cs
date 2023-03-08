using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Edit
{
    /// <summary>
    /// Represents a completion choice returned by the Edit API.  
    /// </summary>
    public class EditChoice
    {
        /// <summary>
        /// The main text of the edit
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; set; }
        /// <summary>
        /// If multiple edit choices we returned, this is the index withing the various choices
        /// </summary>
        [JsonPropertyName("index")]
        public int Index { get; set; }
    }
}
