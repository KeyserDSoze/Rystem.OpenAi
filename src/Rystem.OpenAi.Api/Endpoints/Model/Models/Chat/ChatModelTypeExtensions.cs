namespace Rystem.OpenAi.Models
{
    public static class ChatModelTypeExtensions
    {
        private static readonly Model s_gpt35Turbo = new Model("gpt-3.5-turbo");
        private static readonly Model s_gpt35Turbo0301 = new Model("gpt-3.5-turbo-0301");
        public static Model ToModel(this ChatModelType type)
        {
            switch (type)
            {
                case ChatModelType.Gpt35Turbo:
                    return s_gpt35Turbo;
                default:
                case ChatModelType.Gpt35Turbo0301:
                    return s_gpt35Turbo0301;
            }
        }
    }
}
