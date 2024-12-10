using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public class FunctionToolMainProperty : FunctionToolNonPrimitiveProperty
    {
        public FunctionToolMainProperty() : base()
        {
        }
        [JsonPropertyName("required")]
        public List<string>? RequiredParameters { get; set; }
        [JsonPropertyName("additionalProperties")]
        public bool AdditionalProperties => NumberOfProperties != (RequiredParameters?.Count ?? 0);
    }
    public static class FunctionToolStartPropertyExtensions
    {
        public static T AddRequired<T>(this T functionToolStartProperty,
            params string[] requiredParameters)
            where T : FunctionToolMainProperty
        {
            functionToolStartProperty.RequiredParameters ??= [];
            functionToolStartProperty.RequiredParameters.AddRange(requiredParameters);
            return functionToolStartProperty;
        }
        public static T RemoveRequired<T>(this T functionToolStartProperty,
            params string[] requiredParameters)
            where T : FunctionToolMainProperty
        {
            functionToolStartProperty.RequiredParameters ??= [];
            foreach (var required in requiredParameters)
                functionToolStartProperty.RequiredParameters.Remove(required);
            return functionToolStartProperty;
        }
        public static T ClearRequired<T>(this T functionToolStartProperty)
            where T : FunctionToolMainProperty
        {
            functionToolStartProperty.RequiredParameters ??= [];
            functionToolStartProperty.RequiredParameters.Clear();
            return functionToolStartProperty;
        }
    }
}
