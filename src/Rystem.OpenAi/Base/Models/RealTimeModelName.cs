namespace Rystem.OpenAi
{
    public sealed class RealTimeModelName : ModelName
    {
        internal RealTimeModelName(string name) : base(name) { }
        public static ChatModelName Gpt_4o_realtime_preview_2024_12_17 { get; } = "gpt-4o-realtime-preview-2024-12-17";
        public static ChatModelName Gpt_4o_mini_realtime_preview_2024_12_17 { get; } = "gpt-4o-mini-realtime-preview-2024-12-17";
        public static implicit operator string(RealTimeModelName name)
           => name.Name;
        public static implicit operator RealTimeModelName(string name)
            => new(name);
    }
}
