using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public class FunctionToolNonPrimitiveProperty : FunctionToolProperty
    {
        internal const string DefaultTypeName = "object";
        public FunctionToolNonPrimitiveProperty()
        {
            Type = DefaultTypeName;
            Properties = [];
        }
        [JsonPropertyName("properties")]
        public Dictionary<string, FunctionToolProperty> Properties { get; }
        public FunctionToolNonPrimitiveProperty AddEnum(string key, FunctionToolEnumProperty property)
            => AddProperty(key, property);
        public FunctionToolNonPrimitiveProperty AddObject(string key, FunctionToolNonPrimitiveProperty property)
            => AddProperty(key, property);
        public FunctionToolNonPrimitiveProperty AddPrimitive(string key, FunctionToolProperty property)
            => AddProperty(key, property);
        public FunctionToolNonPrimitiveProperty AddNumber(string key, FunctionToolNumberProperty property)
            => AddProperty(key, property);
        public FunctionToolNonPrimitiveProperty AddArray(string key, FunctionToolArrayProperty property)
            => AddProperty(key, property);
        internal FunctionToolNonPrimitiveProperty AddProperty<T>(string key, T property)
            where T : FunctionToolProperty
        {
            if (!Properties.ContainsKey(key))
                Properties.Add(key, property);
            return this;
        }
    }
}
