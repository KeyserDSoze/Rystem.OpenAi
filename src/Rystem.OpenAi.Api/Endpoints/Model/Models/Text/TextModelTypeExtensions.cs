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
        private static readonly Model s_gpt4 = new Model("gpt-4");
        private static readonly Model s_gpt4_32k = new Model("gpt-4-32k");
        private static readonly Model s_gpt_3_5_turbo = new Model("gpt-3.5-turbo");
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
                case TextModelType.Gpt35Turbo:
                    return s_gpt_3_5_turbo;
                case TextModelType.Gpt4:
                    return s_gpt4;
                case TextModelType.Gpt4_32K:
                    return s_gpt4_32k;
                default:
                case TextModelType.AdaText:
                    return s_adaText;
            }
        }
        public static ModelFamilyType ToFamily(this TextModelType type)
        {
            switch (type)
            {
                case TextModelType.BabbageText:
                    return ModelFamilyType.Babbage;
                case TextModelType.CurieText:
                    return ModelFamilyType.Curie;
                case TextModelType.DavinciText3:
                    return ModelFamilyType.Davinci;
                case TextModelType.DavinciText2:
                    return ModelFamilyType.Davinci;
                case TextModelType.DavinciCode:
                    return ModelFamilyType.Davinci;
                case TextModelType.CushmanCode:
                    return ModelFamilyType.Ada;
                case TextModelType.Gpt35Turbo:
                    return ModelFamilyType.Gpt3_5;
                case TextModelType.Gpt4:
                    return ModelFamilyType.Gpt4_8K;
                case TextModelType.Gpt4_32K:
                    return ModelFamilyType.Gpt4_32K;
                default:
                case TextModelType.AdaText:
                    return ModelFamilyType.Ada;
            }
        }
        public static string ToModelId(this TextModelType type)
            => type.ToModel().Id!;
    }
}
