namespace Rystem.OpenAi
{
    public static class ModerationModelTypeExtensions
    {
        private static readonly Model s_textModerationStable = new Model("text-moderation-stable");
        private static readonly Model s_textModerationLatest = new Model("text-moderation-latest");
        public static Model ToModel(this ModerationModelType type)
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
        public static string ToModelId(this ModerationModelType type)
            => type.ToModel().Id!;
    }
}
