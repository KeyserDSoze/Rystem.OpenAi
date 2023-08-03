namespace System.Text.Json.Serialization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonPropertyDescriptionAttribute : Attribute
    {
        public string Description { get; }
        public JsonPropertyDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
