using System.Text.Json.Serialization;

namespace Rystem.PlayFramework
{
    public sealed class SceneRequestSettingsForApi
    {
        [JsonPropertyName("sta")]
        public List<string>? ScenesToAvoid { get; set; }
        [JsonPropertyName("p")]
        public Dictionary<object, object>? Properties { get; set; }
        [JsonPropertyName("ac")]
        public bool AvoidCache { get; set; }
    }
}
