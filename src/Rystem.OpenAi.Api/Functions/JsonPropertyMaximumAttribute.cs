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
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonPropertyMaximumAttribute : Attribute
    {
        public double Maximum { get; }
        public bool Exclusive { get; }
        public JsonPropertyMaximumAttribute(double maximum, bool exclusive = false)
        {
            Maximum = maximum;
            Exclusive = exclusive;
        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonPropertyMinimumAttribute : Attribute
    {
        public double Minimum { get; }
        public bool Exclusive { get; }
        public JsonPropertyMinimumAttribute(double minimum, bool exclusive = false)
        {
            Minimum = minimum;
            Exclusive = exclusive;
        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonPropertyRangeAttribute : Attribute
    {
        public double Minimum { get; }
        public double Maximum { get; }
        public bool Exclusive { get; }
        public JsonPropertyRangeAttribute(double minimum, double maximum, bool exclusive = false)
        {
            Minimum = minimum;
            Maximum = maximum;
            Exclusive = exclusive;
        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonPropertyMultipleOfAttribute : Attribute
    {
        public double MultipleOf { get; }
        public JsonPropertyMultipleOfAttribute(double multipleOf)
        {
            MultipleOf = multipleOf;
        }
    }
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
