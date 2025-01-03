namespace Rystem.OpenAi.Assistant
{
    public sealed class RankerName : ModelName
    {
        internal RankerName(string name) : base(name) { }
        public static RankerName Auto { get; } = new("auto");
        public static RankerName Default_2024_08_21 { get; } = new("default_2024_08_21");
    }
}
