using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.FineTune
{
    /// <summary>
    /// Manage fine-tuning jobs to tailor a model to your specific training data. See <see href="https://platform.openai.com/docs/guides/fine-tuning">Fine-tuning guide</see>.
    /// </summary>
    public interface IOpenAiFineTuneApi
    {
        /// <summary>
        /// Creates a job that fine-tunes a specified model from a given dataset. Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.
        /// <see href="https://platform.openai.com/docs/guides/fine-tuning">Learn more</see>
        /// </summary>
        /// <param name="trainingFileId"></param>
        /// <returns>Builder</returns>
        FineTuneRequestBuilder Create(string trainingFileId);
        ValueTask<FineTuneResults> ListAsync(CancellationToken cancellationToken = default);
        ValueTask<FineTuneResult> RetrieveAsync(string fineTuneId, CancellationToken cancellationToken = default);
        ValueTask<FineTuneResult> CancelAsync(string fineTuneId, CancellationToken cancellationToken = default);
        ValueTask<FineTuneEventsResult> ListEventsAsync(string fineTuneId, CancellationToken cancellationToken = default);
        ValueTask<FineTuneDeleteResult> DeleteAsync(string fineTuneId, CancellationToken cancellationToken = default);
    }
}
