using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Embedding
{
    public interface IOpenAiEmbedding : IOpenAiBase<IOpenAiEmbedding>
    {
        /// <summary>
        /// Adds inputs for the embedding operation.
        /// </summary>
        /// <param name="inputs">An array of input strings.</param>
        /// <returns>The current instance for method chaining.</returns>
        IOpenAiEmbedding WithInputs(params string[] inputs);

        /// <summary>
        /// Clears all previously added inputs.
        /// </summary>
        /// <returns>The current instance for method chaining.</returns>
        IOpenAiEmbedding ClearInputs();

        /// <summary>
        /// Adds a single prompt to the input collection.
        /// </summary>
        /// <param name="input">A string containing the input prompt.</param>
        /// <returns>The current instance for method chaining.</returns>
        IOpenAiEmbedding AddPrompt(string input);

        /// <summary>
        /// Sets a unique user identifier for the operation.
        /// Helps in monitoring and detecting abuse.
        /// </summary>
        /// <param name="user">A unique identifier for the end-user.</param>
        /// <returns>The current instance for method chaining.</returns>
        IOpenAiEmbedding WithUser(string user);

        /// <summary>
        /// Sets the number of dimensions for the output embeddings.
        /// Supported in specific embedding models only.
        /// </summary>
        /// <param name="dimensions">The desired number of dimensions.</param>
        /// <returns>The current instance for method chaining.</returns>
        IOpenAiEmbedding WithDimensions(int dimensions);

        /// <summary>
        /// Configures the encoding format for the embeddings.
        /// </summary>
        /// <param name="encodingFormat">The encoding format (e.g., Base64 or Float).</param>
        /// <returns>The current instance for method chaining.</returns>
        IOpenAiEmbedding WithEncodingFormat(EncodingFormatForEmbedding encodingFormat);

        /// <summary>
        /// Executes the embedding operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation if required.</param>
        /// <returns>A task representing the result of the embedding operation.</returns>
        ValueTask<EmbeddingResult> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
