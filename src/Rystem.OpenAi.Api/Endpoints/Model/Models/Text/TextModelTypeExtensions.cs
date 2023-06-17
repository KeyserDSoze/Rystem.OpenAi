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
        private static readonly Model s_gpt4_snapshot = new Model("gpt-4-0613");
        private static readonly Model s_gpt4_32k_snapshot = new Model("gpt-4-32k-0613");
        private static readonly Model s_gpt_3_5_turbo = new Model("gpt-3.5-turbo");
        private static readonly Model s_gpt_3_5_turbo_16k = new Model("gpt-3.5-turbo-16k");
        private static readonly Model s_gpt_3_5_turbo_snapshot = new Model("gpt-3.5-turbo-0613");
        private static readonly Model s_gpt_3_5_turbo_16k_snapshot = new Model("gpt-3.5-turbo-16k-0613");
        public static Model ToModel(this TextModelType type)
        {
            return type switch
            {
                TextModelType.BabbageText => s_babbageText,
                TextModelType.CurieText => s_curieText,
                TextModelType.DavinciText3 => s_davinciText3,
                TextModelType.DavinciText2 => s_davinciText2,
                TextModelType.DavinciCode => s_davinciCode,
                TextModelType.CushmanCode => s_cushmanCode,
                TextModelType.Gpt35Turbo => s_gpt_3_5_turbo,
                TextModelType.Gpt35Turbo_16K => s_gpt_3_5_turbo_16k,
                TextModelType.Gpt35Turbo_Snapshot => s_gpt_3_5_turbo_snapshot,
                TextModelType.Gpt35Turbo_16K_Snapshot => s_gpt_3_5_turbo_16k_snapshot,
                TextModelType.Gpt4 => s_gpt4,
                TextModelType.Gpt4_32K => s_gpt4_32k,
                TextModelType.Gpt4_Snapshot => s_gpt4_snapshot,
                TextModelType.Gpt4_32K_Snapshot => s_gpt4_32k_snapshot,
                _ => s_adaText,
            };
        }
        public static ModelFamilyType ToFamily(this TextModelType type)
        {
            return type switch
            {
                TextModelType.BabbageText => ModelFamilyType.Babbage,
                TextModelType.CurieText => ModelFamilyType.Curie,
                TextModelType.DavinciText3 => ModelFamilyType.Davinci,
                TextModelType.DavinciText2 => ModelFamilyType.Davinci,
                TextModelType.DavinciCode => ModelFamilyType.Davinci,
                TextModelType.CushmanCode => ModelFamilyType.Ada,
                TextModelType.Gpt35Turbo => ModelFamilyType.Gpt3_5,
                TextModelType.Gpt35Turbo_16K => ModelFamilyType.Gpt3_5_16K,
                TextModelType.Gpt35Turbo_Snapshot => ModelFamilyType.Gpt3_5,
                TextModelType.Gpt35Turbo_16K_Snapshot => ModelFamilyType.Gpt3_5_16K,
                TextModelType.Gpt4 => ModelFamilyType.Gpt4_8K,
                TextModelType.Gpt4_32K => ModelFamilyType.Gpt4_32K,
                TextModelType.Gpt4_Snapshot => ModelFamilyType.Gpt4_8K,
                TextModelType.Gpt4_32K_Snapshot => ModelFamilyType.Gpt4_32K,
                _ => ModelFamilyType.Ada,
            };
        }
        public static string ToModelId(this TextModelType type)
            => type.ToModel().Id!;
    }
}
