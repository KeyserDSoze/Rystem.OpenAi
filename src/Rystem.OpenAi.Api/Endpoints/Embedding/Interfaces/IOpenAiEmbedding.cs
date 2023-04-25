using System;

namespace Rystem.OpenAi.Embedding
{
    public interface IOpenAiEmbedding
    {
        /// <summary>
        /// Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.
        /// <see href="https://platform.openai.com/docs/guides/embeddings">Guide</see>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        EmbeddingRequestBuilder Request(params string[] inputs);
    }
}
