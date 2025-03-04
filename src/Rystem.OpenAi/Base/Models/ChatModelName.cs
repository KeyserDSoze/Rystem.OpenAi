namespace Rystem.OpenAi
{
    public sealed class ChatModelName : ModelName
    {
        internal ChatModelName(string name) : base(name) { }
        public static ChatModelName Gpt_4o { get; } = "gpt-4o";
        public static ChatModelName Gpt_4o_2024_11_20 { get; } = "gpt-4o-2024-11-20";
        public static ChatModelName Gpt_4o_2024_08_06 { get; } = "gpt-4o-2024-08-06";
        public static ChatModelName Gpt_4o_audio_preview { get; } = "gpt-4o-audio-preview";
        public static ChatModelName Gpt_4o_audio_preview_2024_10_01 { get; } = "gpt-4o-audio-preview-2024-10-01";
        public static ChatModelName Gpt_4o_2024_05_13 { get; } = "gpt-4o-2024-05-13";
        public static ChatModelName Gpt_4o_mini { get; } = "gpt-4o-mini";
        public static ChatModelName Gpt_4o_mini_2024_07_18 { get; } = "gpt-4o-mini-2024-07-18";
        public static ChatModelName O1_preview { get; } = "o1-preview";
        public static ChatModelName O1_preview_2024_09_12 { get; } = "o1-preview-2024-09-12";
        public static ChatModelName O1_mini { get; } = "o1-mini";
        public static ChatModelName O1_mini_2024_09_12 { get; } = "o1-mini-2024-09-12";
        public static ChatModelName Gpt_4o_latest { get; } = "gpt-4o-latest";
        public static ChatModelName Gpt_4_turbo { get; } = "gpt-4-turbo";
        public static ChatModelName Gpt_4_turbo_2024_04_09 { get; } = "gpt-4-turbo-2024-04-09";
        public static ChatModelName Gpt_4 { get; } = "gpt-4";
        public static ChatModelName Gpt_4_32k { get; } = "gpt-4-32k";
        public static ChatModelName Gpt_4_0125_preview { get; } = "gpt-4-0125-preview";
        public static ChatModelName Gpt_4_1106_preview { get; } = "gpt-4-1106-preview";
        public static ChatModelName Gpt_4_vision_preview { get; } = "gpt-4-vision-preview";
        public static implicit operator string(ChatModelName name)
            => name.Name;
        public static implicit operator ChatModelName(string name)
            => new(name);
    }
}
