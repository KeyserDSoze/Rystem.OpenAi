namespace Rystem.OpenAi.Models
{
    /// <summary>
    /// Embeddings are a numerical representation of text that can be used to measure the relateness between two pieces of text. Our second generation embedding model, text-embedding-ada-002 is a designed to replace the previous 16 first-generation embedding models at a fraction of the cost. Embeddings are useful for search, clustering, recommendations, anomaly detection, and classification tasks. You can read more about our latest embedding model in the <see href="https://openai.com/blog/new-and-improved-embedding-model">announcement blog post</see>.
    /// </summary>
    public enum EmbeddingModelType
    {
        AdaTextEmbedding,
    }
}
