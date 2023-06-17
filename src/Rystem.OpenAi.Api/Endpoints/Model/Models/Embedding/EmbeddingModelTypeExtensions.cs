namespace Rystem.OpenAi
{
    public static class EmbeddingModelTypeExtensions
    {
        private static readonly Model s_adaTextEmbedding = new Model("text-embedding-ada-002");
        public static Model ToModel(this EmbeddingModelType type)
        {
            return type switch
            {
                _ => s_adaTextEmbedding,
            };
        }
        public static ModelFamilyType ToFamily(this EmbeddingModelType type)
        {
            return type switch
            {
                _ => ModelFamilyType.Ada,
            };
        }
        public static string ToModelId(this EmbeddingModelType type)
            => type.ToModel().Id!;
    }
}
