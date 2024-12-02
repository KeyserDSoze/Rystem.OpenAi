namespace Rystem.OpenAi
{
    public sealed class ModerationModelName : ModelName
    {
        internal ModerationModelName(string name) : base(name) { }
        public static ModerationModelName OmniLatest { get; } = "omni-moderation-latest";
        public static implicit operator string(ModerationModelName name)
            => name.Name;
        public static implicit operator ModerationModelName(string name)
            => new(name);
    }
}
