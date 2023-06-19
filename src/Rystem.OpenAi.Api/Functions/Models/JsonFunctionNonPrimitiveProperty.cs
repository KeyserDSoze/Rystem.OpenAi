using System.Collections.Generic;

namespace System.Text.Json.Serialization
{
    public sealed class JsonFunctionNonPrimitiveProperty : JsonFunctionProperty
    {
        private const string DefaultTypeName = "object";
        public JsonFunctionNonPrimitiveProperty()
        {
            Type = DefaultTypeName;
            Properties = new Dictionary<string, JsonFunctionProperty>();
        }
        [JsonPropertyName("properties")]
        public Dictionary<string, JsonFunctionProperty> Properties { get; }
        [JsonPropertyName("required")]
        public List<string>? Required { get; private set; }
        public JsonFunctionNonPrimitiveProperty AddRequired(params string[] names)
        {
            Required ??= new List<string>();
            Required.AddRange(names);
            return this;
        }
        public JsonFunctionNonPrimitiveProperty AddEnum(string key, JsonFunctionEnumProperty property)
            => AddProperty(key, property);
        public JsonFunctionNonPrimitiveProperty AddObject(string key, JsonFunctionNonPrimitiveProperty property)
            => AddProperty(key, property);
        public JsonFunctionNonPrimitiveProperty AddPrimitive(string key, JsonFunctionProperty property)
            => AddProperty(key, property);
        public JsonFunctionNonPrimitiveProperty AddNumber(string key, JsonFunctionNumberProperty property)
            => AddProperty(key, property);
        public JsonFunctionNonPrimitiveProperty AddArray(string key, JsonFunctionArrayProperty property)
            => AddProperty(key, property);
        internal JsonFunctionNonPrimitiveProperty AddProperty<T>(string key, T property)
            where T : JsonFunctionProperty
        {
            if (!Properties.ContainsKey(key))
                Properties.Add(key, property);
            return this;
        }
    }
}
