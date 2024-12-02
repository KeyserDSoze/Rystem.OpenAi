using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public sealed class ToolMainProperty : FunctionToolNonPrimitiveProperty
    {
        public ToolMainProperty() : base() { }
        [JsonPropertyName("required")]
        public List<string>? Required { get; private set; }
        [JsonPropertyName("additionalProperties")]
        public bool AdditionalProperties { get; set; }
        public FunctionToolNonPrimitiveProperty AddRequired(params string[] names)
        {
            Required ??= new List<string>();
            Required.AddRange(names);
            return this;
        }
    }
}
