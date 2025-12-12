using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public sealed class FunctionToolEnumProperty : FunctionToolProperty
    {
        private const string DefaultTypeName = "string";
        public FunctionToolEnumProperty()
        {
            Type = DefaultTypeName;
        }
        [JsonPropertyName("enum")]
        public List<string>? Enums { get; set; }
    }
}

