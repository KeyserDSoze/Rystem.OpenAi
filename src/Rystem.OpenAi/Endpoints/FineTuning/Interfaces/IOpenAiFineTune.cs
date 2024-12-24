using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.FineTune
{
    /// <summary>
    /// Manage fine-tuning jobs to tailor a model to your specific training data. See <see href="https://platform.openai.com/docs/guides/fine-tuning">Fine-tuning guide</see>.
    /// </summary>
    public interface IOpenAiFineTune : IOpenAiBase<IOpenAiFineTune, FineTuningModelName>
    {
        /// <summary>
        /// Sets the file ID of the training dataset.
        /// </summary>
        /// <param name="trainingFileId">The ID of the training file.</param>
        /// <returns>The current builder instance.</returns>
        IOpenAiFineTune WithFileId(string trainingFileId);

        /// <summary>
        /// Lists fine-tuning results.
        /// </summary>
        /// <param name="take">The number of results to take.</param>
        /// <param name="skip">The number of results to skip.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing fine-tune results.</returns>
        ValueTask<FineTuneResults> ListAsync(int take = 20, int skip = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a fine-tune result by ID.
        /// </summary>
        /// <param name="fineTuneId">The ID of the fine-tune operation.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing the fine-tune result.</returns>
        ValueTask<FineTuneResult> RetrieveAsync(string fineTuneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels a fine-tune operation by ID.
        /// </summary>
        /// <param name="fineTuneId">The ID of the fine-tune operation.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing the cancellation result.</returns>
        ValueTask<FineTuneResult> CancelAsync(string fineTuneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists checkpoint events for a fine-tune operation.
        /// </summary>
        /// <param name="fineTuneId">The ID of the fine-tune operation.</param>
        /// <param name="take">The number of results to take.</param>
        /// <param name="skip">The number of results to skip.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing the checkpoint events.</returns>
        ValueTask<ResponseAsArray<FineTuneCheckPointResult>> CheckPointEventsAsync(string fineTuneId, int take = 20, int skip = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists events for a fine-tune operation.
        /// </summary>
        /// <param name="fineTuneId">The ID of the fine-tune operation.</param>
        /// <param name="take">The number of results to take.</param>
        /// <param name="skip">The number of results to skip.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing the events.</returns>
        ValueTask<ResponseAsArray<FineTuneEvent>> ListEventsAsync(string fineTuneId, int take = 20, int skip = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Streams fine-tune results as an asynchronous enumerable.
        /// </summary>
        /// <param name="take">The number of results to take.</param>
        /// <param name="skip">The number of results to skip.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An asynchronous enumerable of fine-tune results.</returns>
        IAsyncEnumerable<FineTuneResult> ListAsStreamAsync(int take = 20, int skip = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Streams events for a fine-tune operation as an asynchronous enumerable.
        /// </summary>
        /// <param name="fineTuneId">The ID of the fine-tune operation.</param>
        /// <param name="take">The number of results to take.</param>
        /// <param name="skip">The number of results to skip.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An asynchronous enumerable of fine-tune events.</returns>
        IAsyncEnumerable<FineTuneEvent> ListEventsAsStreamAsync(string fineTuneId, int take = 20, int skip = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the fine-tune operation.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing the execution result.</returns>
        ValueTask<FineTuneResult> ExecuteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the ID of a validation file for the fine-tune operation.
        /// </summary>
        /// <param name="validationFileId">The ID of the validation file.</param>
        /// <returns>The current builder instance.</returns>
        IOpenAiFineTune WithValidationFile(string validationFileId);

        /// <summary>
        /// Configures hyperparameters for the fine-tune operation.
        /// </summary>
        /// <param name="hyperParametersSettings">A delegate to configure the hyperparameters.</param>
        /// <returns>The current builder instance.</returns>
        IOpenAiFineTune WithHyperParameters(Action<FineTuneHyperParameters> hyperParametersSettings);

        /// <summary>
        /// Adds a suffix to the fine-tuned model name.
        /// </summary>
        /// <param name="value">The suffix string.</param>
        /// <returns>The current builder instance.</returns>
        IOpenAiFineTune WithSuffix(string value);

        /// <summary>
        /// Sets the seed value for reproducibility.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        /// <returns>The current builder instance.</returns>
        IOpenAiFineTune WithSeed(int seed);

        /// <summary>
        /// Adds a specific integration for Weights and Biases.
        /// </summary>
        /// <param name="integration">A delegate to configure the integration.</param>
        /// <returns>The current builder instance.</returns>
        IOpenAiFineTune WithSpecificWeightAndBiasesIntegration(Action<WeightsAndBiasesFineTuneIntegration> integration);

        /// <summary>
        /// Clears all integrations for the fine-tune operation.
        /// </summary>
        /// <returns>The current builder instance.</returns>
        IOpenAiFineTune ClearIntegrations();
    }
}
