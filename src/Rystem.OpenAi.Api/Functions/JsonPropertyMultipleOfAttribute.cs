namespace System.Text.Json.Serialization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonPropertyMultipleOfAttribute : Attribute
    {
        public double MultipleOf { get; }
        public JsonPropertyMultipleOfAttribute(double multipleOf)
        {
            MultipleOf = multipleOf;
        }
    }
}
