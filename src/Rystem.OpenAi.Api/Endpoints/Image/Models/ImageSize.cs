namespace Rystem.OpenAi.Image
{
    public enum ImageSize
    {
        /// <summary>
        /// 256x256
        /// </summary>
        Small,
        /// <summary>
        /// 512x512
        /// </summary>
        Medium,
        /// <summary>
        /// 1024x1024
        /// </summary>
        Large,
    }
    public static class ImageSizeExtensions
    {
        private const string SmallSize = "256x256";
        private const string MediumSize = "512x512";
        private const string LargeSize = "1024x1024";
        public static string AsString(this ImageSize size)
        {
            switch (size)
            {
                case ImageSize.Small:
                    return SmallSize;
                case ImageSize.Medium:
                    return MediumSize;
                default:
                case ImageSize.Large:
                    return LargeSize;
            }
        }
    }
}
