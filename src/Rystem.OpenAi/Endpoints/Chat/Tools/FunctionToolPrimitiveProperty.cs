using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public sealed class FunctionToolPrimitiveProperty : FunctionToolProperty
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        public FunctionToolPrimitiveProperty() : base()
        {
        }
    }
}
