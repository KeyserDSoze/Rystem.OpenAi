namespace System.Text.Json.Serialization
{
    public sealed class JsonFunctionNumberProperty : JsonFunctionProperty
    {
        private const string DefaultTypeName = "number";
        public JsonFunctionNumberProperty()
        {
            Type = DefaultTypeName;
        }
        [JsonPropertyName("multipleOf")]
        public double? MultipleOf { get; set; }
        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }
        [JsonPropertyName("exclusiveMinimum")]
        public bool? ExclusiveMinimum { get; set; }
        [JsonPropertyName("exclusiveMaximum")]
        public bool? ExclusiveMaximum { get; set; }
    }
}
