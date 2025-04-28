namespace Rystem.OpenAi.Image
{
    public static class ImageExtensions
    {
        private const string SmallSize = "256x256";
        private const string MediumSize = "512x512";
        private const string LargeSize = "1024x1024";
        private const string WideLandscapeSize = "1792x1024";
        private const string WidePortraitSize = "1024x1792";
        private const string HdLabel = "hd";
        private const string StandardLabel = "standard";
        private const string HighLabel = "high";
        private const string MediumLabel = "medium";
        private const string LowLabel = "low";
        private const string AutoLabel = "auto";
        private const string VividLabel = "vivid";
        private const string NaturalLabel = "natural";
        private const string JpegLabel = "jpeg";
        private const string PngLabel = "png";
        private const string WebpLabel = "webp";
        private const string TransparentLabel = "transparent";
        private const string OpaqueLabel = "opaque";

        public static string AsString(this ImageSize size)
        {
            return size switch
            {
                ImageSize.Small => SmallSize,
                ImageSize.Medium => MediumSize,
                ImageSize.Large => LargeSize,
                ImageSize.WidePortrait => WidePortraitSize,
                _ => WideLandscapeSize,
            };
        }
        public static string AsString(this ImageQuality quality)
        {
            return quality switch
            {
                ImageQuality.Hd => HdLabel,
                ImageQuality.Standard => StandardLabel,
                ImageQuality.High => HighLabel,
                ImageQuality.Medium => MediumLabel,
                ImageQuality.Low => LowLabel,
                _ => AutoLabel,
            };
        }
        public static string AsString(this ImageStyle style)
        {
            return style switch
            {
                ImageStyle.Vivid => VividLabel,
                _ => NaturalLabel,
            };
        }
        public static string AsString(this ImageOutputFormat format)
        {
            return format switch
            {
                ImageOutputFormat.Jpeg => JpegLabel,
                ImageOutputFormat.Png => PngLabel,
                _ => WebpLabel,
            };
        }
        public static string AsString(this ImageBackgroundFormat format)
        {
            return format switch
            {
                ImageBackgroundFormat.Transparent => TransparentLabel,
                ImageBackgroundFormat.Opaque => OpaqueLabel,
                _ => AutoLabel,
            };
        }
        public static KindOfCost AsCost(this ImageSize size, ImageQuality quality)
        {
            return (size, quality) switch
            {
                (ImageSize.Small, ImageQuality.Standard) => KindOfCost.ImageStandard256,
                (ImageSize.Medium, ImageQuality.Standard) => KindOfCost.ImageStandard512,
                (ImageSize.Large, ImageQuality.Standard) => KindOfCost.ImageStandard1024,
                (ImageSize.WidePortrait, ImageQuality.Standard) => KindOfCost.ImageStandard1792x1024,
                (ImageSize.WideLandscape, ImageQuality.Standard) => KindOfCost.ImageStandard1024x1792,
                (ImageSize.Large, ImageQuality.Hd) => KindOfCost.ImageHd1024,
                (ImageSize.WidePortrait, ImageQuality.Hd) => KindOfCost.ImageHd1792x1024,
                _ => KindOfCost.ImageHd1024x1792
            };
        }
    }
}
