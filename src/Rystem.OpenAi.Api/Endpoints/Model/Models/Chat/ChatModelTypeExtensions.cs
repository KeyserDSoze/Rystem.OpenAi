namespace Rystem.OpenAi
{
    public static class ChatModelTypeExtensions
    {
        private static readonly Model s_gpt35Turbo = new Model("gpt-3.5-turbo");
        private static readonly Model s_gpt35Turbo0301 = new Model("gpt-3.5-turbo-0301");
        private static readonly Model s_gpt4 = new Model("gpt-4");
        private static readonly Model s_gpt4_0314 = new Model("gpt-4-0314");
        private static readonly Model s_gpt4_32k = new Model("gpt-4-32k");
        private static readonly Model s_gpt4_32k_0314 = new Model("gpt-4-32k-0314");
        public static Model ToModel(this ChatModelType type)
        {
            switch (type)
            {
                case ChatModelType.Gpt35Turbo:
                    return s_gpt35Turbo;
                case ChatModelType.Gpt4:
                    return s_gpt4;
                case ChatModelType.Gpt4_0314:
                    return s_gpt4_0314;
                case ChatModelType.Gpt4_32K:
                    return s_gpt4_32k;
                case ChatModelType.Gpt4_32K_0314:
                    return s_gpt4_32k_0314;
                default:
                case ChatModelType.Gpt35Turbo0301:
                    return s_gpt35Turbo0301;
            }
        }
        public static ModelFamilyType ToFamily(this ChatModelType type)
        {
            switch (type)
            {
                case ChatModelType.Gpt35Turbo:
                    return ModelFamilyType.Gpt3_5;
                case ChatModelType.Gpt4:
                    return ModelFamilyType.Gpt4_8K;
                case ChatModelType.Gpt4_0314:
                    return ModelFamilyType.Gpt4_8K;
                case ChatModelType.Gpt4_32K:
                    return ModelFamilyType.Gpt4_32K;
                case ChatModelType.Gpt4_32K_0314:
                    return ModelFamilyType.Gpt4_32K;
                default:
                case ChatModelType.Gpt35Turbo0301:
                    return ModelFamilyType.Gpt3_5;
            }
        }
        public static string ToModelId(this ChatModelType type)
            => type.ToModel().Id!;
    }
}
