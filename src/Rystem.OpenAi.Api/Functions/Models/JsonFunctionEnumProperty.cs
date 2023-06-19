using System.Collections.Generic;

namespace System.Text.Json.Serialization
{
    public sealed class JsonFunctionEnumProperty : JsonFunctionProperty
    {
        private const string DefaultTypeName = "string";
        public JsonFunctionEnumProperty()
        {
            Type = DefaultTypeName;
        }
        [JsonPropertyName("enum")]
        public List<string>? Enums { get; set; }
    }
}
