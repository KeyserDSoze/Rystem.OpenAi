namespace Rystem.OpenAi
{
    public sealed class ImageModelName : ModelName
    {
        internal ImageModelName(string name) : base(name) { }
        public static ImageModelName GptImage1 { get; } = "gpt-image-1";
        public static ImageModelName Dalle3 { get; } = "dall-e-3";
        public static ImageModelName Dalle2 { get; } = "dall-e-2";
        public static implicit operator string(ImageModelName name)
            => name.Name;
        public static implicit operator ImageModelName(string name)
            => new(name);
    }
}
