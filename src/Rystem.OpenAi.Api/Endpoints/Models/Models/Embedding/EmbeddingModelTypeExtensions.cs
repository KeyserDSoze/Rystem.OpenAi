namespace Rystem.OpenAi
{
    public static class EmbeddingModelTypeExtensions
    {
        private static readonly string s_adaTextEmbedding = "text-embedding-ada-002";
        public static string ToModel(this EmbeddingModelType type)
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
    }
}
