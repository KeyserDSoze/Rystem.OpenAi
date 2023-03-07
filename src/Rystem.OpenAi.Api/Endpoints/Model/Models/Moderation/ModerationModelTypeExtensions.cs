namespace Rystem.OpenAi
{
    public static class ModerationModelTypeExtensions
    {
        private static readonly Model s_textModerationStable = new Model("text-moderation-stable");
        private static readonly Model s_textModerationLatest = new Model("text-moderation-latest");
        public static Model ToModel(this ModerationModelType type)
        {
            switch (type)
            {
                case ModerationModelType.TextModerationStable:
                    return s_textModerationStable;
                default:
                case ModerationModelType.TextModerationLatest:
                    return s_textModerationLatest;
            }
        }
        public static string ToModelId(this ModerationModelType type)
            => type.ToModel().Id!;
    }
}
