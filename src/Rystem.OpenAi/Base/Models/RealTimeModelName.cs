namespace Rystem.OpenAi
{
    public sealed class RealTimeModelName : ModelName
    {
        internal RealTimeModelName(string name) : base(name) { }
        public static RealTimeModelName Gpt_4o_realtime_preview { get; } = "gpt-4o-real-time-preview";
        public static RealTimeModelName Gpt_4o_realtime_preview_2024_10_01 { get; } = "gpt-4o-real-time-preview-2024-10-01";
        public static implicit operator string(RealTimeModelName name)
            => name.Name;
        public static implicit operator RealTimeModelName(string name)
            => new(name);
    }
}
