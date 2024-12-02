namespace Rystem.OpenAi
{
    public sealed class EmbeddingModelName : ModelName
    {
        internal EmbeddingModelName(string name) : base(name) { }
        public static EmbeddingModelName Text_embedding_3_small { get; } = "text-embedding-3-small";
        public static EmbeddingModelName Text_embedding_3_large { get; } = "text-embedding-3-large";
        public static EmbeddingModelName AdaV2 { get; } = "ada v2";
        public static implicit operator string(EmbeddingModelName name)
            => name.Name;
        public static implicit operator EmbeddingModelName(string name)
            => new(name);
    }
}
