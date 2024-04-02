using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public abstract class OpenAiList<T>
    {
        private const string ObjectLabel = "list";
        [JsonPropertyName("object")]
        public string Object => ObjectLabel;
        [JsonPropertyName("data")]
        public List<T>? Items { get; set; }
    }
}
