using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi
{
    /// <summary>
    /// List and describe the various models available in the API. 
    /// You can refer to the <see href="https://platform.openai.com/docs/models">Models documentation</see> to understand what models are available and the differences between them.
    /// </summary>
    public interface IOpenAiModel
    {
        /// <summary>
        /// Get details about a particular Model from the API, specifically properties such as <see cref="Model.OwnedBy"/> and permissions.
        /// </summary>
        /// <param name="id">The id/name of the model to get more details about</param>
        /// <returns>Asynchronously returns the <see cref="Model"/> with all available properties</returns>
        ValueTask<Model> RetrieveAsync(string id, CancellationToken cancellationToken = default);
        /// <summary>
        /// List all models via the API
        /// </summary>
        /// <returns>Asynchronously returns the list of all <see cref="Model"/>s</returns>
        Task<List<Model>> ListAsync(CancellationToken cancellationToken = default);
    }
    [Obsolete("In version 3.x we'll remove IOpenAiModelApi and we'll use only IOpenAiModel to retrieve services")]
    public interface IOpenAiModelApi : IOpenAiModel
    {
    }
}
