using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public abstract class OpenAiObject
    {
        private const string ObjectLabel = "object";
        [JsonPropertyName("object")]
        public string Object => ObjectLabel;
    }
}
