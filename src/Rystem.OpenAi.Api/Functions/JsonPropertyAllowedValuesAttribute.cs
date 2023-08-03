namespace System.Text.Json.Serialization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonPropertyAllowedValuesAttribute : Attribute
    {
        public string[] Values { get; }
        public JsonPropertyAllowedValuesAttribute(params string[] values)
        {
            Values = values;
        }
    }
}
