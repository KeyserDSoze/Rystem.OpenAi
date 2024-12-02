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
        private const string VividLabel = "vivid";
        private const string NaturalLabel = "natural";

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
                _ => StandardLabel,
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
