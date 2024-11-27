namespace Rystem.OpenAi
{
    public sealed class SpeechModelName : ModelName
    {
        internal SpeechModelName(string name) : base(name) { }
        public static SpeechModelName Tts { get; } = "tts-1";
        public static SpeechModelName TtsHd { get; } = "tts-1-hd";
        public static implicit operator string(SpeechModelName name)
            => name.Name;
        public static implicit operator SpeechModelName(string name)
            => new(name);
    }
}
