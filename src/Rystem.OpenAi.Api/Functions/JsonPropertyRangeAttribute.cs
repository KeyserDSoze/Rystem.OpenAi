namespace System.Text.Json.Serialization
{
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
}
