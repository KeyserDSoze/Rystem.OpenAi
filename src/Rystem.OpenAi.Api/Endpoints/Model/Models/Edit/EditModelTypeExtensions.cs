namespace Rystem.OpenAi
{
    public static class EditModelTypeExtensions
    {
        private static readonly Model s_textDavinciEdit = new Model("text-davinci-edit-001");
        private static readonly Model s_codeDavinciEdit = new Model("code-davinci-edit-001");
        public static Model ToModel(this EditModelType type)
        {
            switch (type)
            {
                case EditModelType.TextDavinciEdit:
                    return s_textDavinciEdit;
                default:
                case EditModelType.CodeDavinciEdit:
                    return s_codeDavinciEdit;
            }
        }
        public static string ToModelId(this EditModelType type)
            => type.ToModel().Id!;
    }
}
