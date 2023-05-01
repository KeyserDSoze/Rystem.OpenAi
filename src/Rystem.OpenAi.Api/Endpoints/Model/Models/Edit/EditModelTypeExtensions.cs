namespace Rystem.OpenAi
{
    public static class EditModelTypeExtensions
    {
        private static readonly Model s_textDavinciEdit = new Model("text-davinci-edit-001");
        private static readonly Model s_codeDavinciEdit = new Model("code-davinci-edit-001");
        public static Model ToModel(this EditModelType type)
        {
            return type switch
            {
                EditModelType.TextDavinciEdit => s_textDavinciEdit,
                _ => s_codeDavinciEdit,
            };
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Unused parameter is necessary to work as extension method.")]
        public static ModelFamilyType ToFamily(this EditModelType type)
            => ModelFamilyType.Davinci;
        public static string ToModelId(this EditModelType type)
            => type.ToModel().Id!;
    }
}
