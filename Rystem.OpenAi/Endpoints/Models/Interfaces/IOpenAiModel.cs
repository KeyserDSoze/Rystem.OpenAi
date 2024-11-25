﻿using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.FineTune;

namespace Rystem.OpenAi.Models
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
        Task<ModelListResult> ListAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fineTuneId">The id/name of the fine tune model.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Result for deletion</returns>
        ValueTask<FineTuningDeleteResult> DeleteAsync(string fineTuneId, CancellationToken cancellationToken = default);
    }
}
