namespace System.Text.Json.Serialization
{
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
}
