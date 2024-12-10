using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public class FunctionToolNonPrimitiveProperty : FunctionToolProperty
    {
        internal int _numberOfProperties;
        internal const string DefaultTypeName = "object";
        public FunctionToolNonPrimitiveProperty()
        {
            Type = DefaultTypeName;
            Properties = [];
        }
        [JsonPropertyName("properties")]
        public Dictionary<string, FunctionToolProperty> Properties { get; }
    }
    public static class FunctionToolNonPrimitivePropertyExtensions
    {
        public static T AddEnum<T>(this T functionTool, string key, FunctionToolEnumProperty property)
            where T : FunctionToolNonPrimitiveProperty
            => functionTool.AddProperty(key, property);
        public static T AddObject<T>(this T functionTool, string key, FunctionToolNonPrimitiveProperty property)
            where T : FunctionToolNonPrimitiveProperty
            => functionTool.AddProperty(key, property);
        public static T AddPrimitive<T>(this T functionTool, string key, FunctionToolPrimitiveProperty property)
            where T : FunctionToolNonPrimitiveProperty
            => functionTool.AddProperty(key, property);
        public static T AddNumber<T>(this T functionTool, string key, FunctionToolNumberProperty property)
            where T : FunctionToolNonPrimitiveProperty
            => functionTool.AddProperty(key, property);
        public static T AddArray<T>(this T functionTool, string key, FunctionToolArrayProperty property)
            where T : FunctionToolNonPrimitiveProperty
            => functionTool.AddProperty(key, property);
        private static TOut AddProperty<T, TOut>(this TOut functionTool, string key, T property)
            where T : FunctionToolProperty
            where TOut : FunctionToolNonPrimitiveProperty
        {
            if (!functionTool.Properties.ContainsKey(key))
            {
                functionTool.Properties.Add(key, property);
                functionTool._numberOfProperties++;
            }
            return functionTool;
        }
    }
}
