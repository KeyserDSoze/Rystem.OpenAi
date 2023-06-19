namespace System.Text.Json.Serialization
{
    public sealed class JsonFunction
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;
        [JsonPropertyName("parameters")]
        public JsonFunctionNonPrimitiveProperty Parameters { get; set; } = null!;
    }
}
