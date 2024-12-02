namespace Rystem.OpenAi
{
    public sealed class FineTuningModelName : ModelName
    {
        internal FineTuningModelName(string name) : base(name) { }
        public static FineTuningModelName Gpt_4o_2024_08_06 { get; } = "gpt-4o-2024-08-06";
        public static FineTuningModelName Gpt_4o_mini_2024_07_18 { get; } = "gpt-4o-mini-2024-07-18";
        public static FineTuningModelName Gpt_35_turbo { get; } = "gpt-3.5-turbo";
        public static FineTuningModelName Gpt_35_turbo_0125 { get; } = "gpt-3.5-turbo-0125";
        public static FineTuningModelName Gpt_35_turbo_instruct { get; } = "gpt-3.5-turbo-instruct";
        public static FineTuningModelName Gpt_35_turbo_1106 { get; } = "gpt-3.5-turbo-1106";
        public static FineTuningModelName Gpt_35_turbo_0613 { get; } = "gpt-3.5-turbo-0613";
        public static FineTuningModelName Gpt_35_turbo_16k_0613 { get; } = "gpt-3.5-turbo-16k-0613";
        public static FineTuningModelName Gpt_35_turbo_0301 { get; } = "gpt-3.5-turbo-0301";
        public static FineTuningModelName Davinci_002 { get; } = "DaVinci-002";
        public static FineTuningModelName Babbage_002 { get; } = "Babbage-002";
        public static implicit operator string(FineTuningModelName name)
            => name.Name;
        public static implicit operator FineTuningModelName(string name)
            => new(name);
    }
}
