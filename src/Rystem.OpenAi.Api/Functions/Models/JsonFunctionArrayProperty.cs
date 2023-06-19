namespace System.Text.Json.Serialization
{
    public sealed class JsonFunctionArrayProperty : JsonFunctionProperty
    {
        [JsonPropertyName("items")]
        public JsonFunctionProperty? Items { get; set; }
    }
}
