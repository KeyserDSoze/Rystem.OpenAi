namespace Rystem.OpenAi
{
    public static class TextModelTypeExtensions
    {
        private static readonly Model s_adaText = new Model("text-ada-001");
        private static readonly Model s_babbageText = new Model("text-babbage-001");
        private static readonly Model s_curieText = new Model("text-curie-001");
        private static readonly Model s_davinciText3 = new Model("text-davinci-003");
        private static readonly Model s_davinciText2 = new Model("text-davinci-002");
        private static readonly Model s_davinciCode = new Model("code-davinci-002");
        private static readonly Model s_cushmanCode = new Model("code-cushman-001");
        public static Model ToModel(this TextModelType type)
        {
            switch (type)
            {
                case TextModelType.BabbageText:
                    return s_babbageText;
                case TextModelType.CurieText:
                    return s_curieText;
                case TextModelType.DavinciText3:
                    return s_davinciText3;
                case TextModelType.DavinciText2:
                    return s_davinciText2;
                case TextModelType.DavinciCode:
                    return s_davinciCode;
                case TextModelType.CushmanCode:
                    return s_cushmanCode;
                default:
                case TextModelType.AdaText:
                    return s_adaText;
            }
        }
        public static string ToModelId(this TextModelType type)
            => type.ToModel().Id!;
    }
}
