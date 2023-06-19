namespace System.Text.Json.Serialization
{
    [JsonDerivedType(typeof(JsonFunctionEnumProperty))]
    [JsonDerivedType(typeof(JsonFunctionNumberProperty))]
    [JsonDerivedType(typeof(JsonFunctionNonPrimitiveProperty))]
    public class JsonFunctionProperty
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        private const string DefaultTypeName = "string";
        public JsonFunctionProperty()
        {
            Type = DefaultTypeName;
        }
    }
}
