namespace Rystem.OpenAi
{
    public static class ModerationModelTypeExtensions
    {
        private static readonly string s_textModerationStable = "text-moderation-stable";
        private static readonly string s_textModerationLatest = "text-moderation-latest";
        public static string ToModel(this ModerationModelType type)
        {
            return type switch
            {
                ModerationModelType.TextModerationStable => s_textModerationStable,
                _ => s_textModerationLatest,
            };
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Unused parameter is necessary to work as extension method.")]
        public static ModelFamilyType ToFamily(this ModerationModelType type)
            => ModelFamilyType.Moderation;
    }
}
