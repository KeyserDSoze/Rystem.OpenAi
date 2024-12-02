namespace Rystem.OpenAi
{
    public sealed class AudioModelName : ModelName
    {
        public static AudioModelName Whisper { get; } = "Whisper";
        internal AudioModelName(string name) : base(name) { }

        public static implicit operator string(AudioModelName name)
            => name.Name;
        public static implicit operator AudioModelName(string name)
            => new(name);
    }
}
