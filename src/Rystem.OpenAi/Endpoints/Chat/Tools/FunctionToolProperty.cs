using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    [JsonDerivedType(typeof(FunctionToolEnumProperty))]
    [JsonDerivedType(typeof(FunctionToolNumberProperty))]
    [JsonDerivedType(typeof(FunctionToolNonPrimitiveProperty))]
    [JsonDerivedType(typeof(FunctionToolArrayProperty))]
    public class FunctionToolProperty
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        private const string DefaultTypeName = "string";
        public FunctionToolProperty()
        {
            Type = DefaultTypeName;
        }
    }
}
