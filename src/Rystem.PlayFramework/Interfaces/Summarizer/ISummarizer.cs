namespace Rystem.PlayFramework
{
    /// <summary>
    /// Defines the summarizer for condensing conversation history.
    /// </summary>
    public interface ISummarizer
    {
        /// <summary>
        /// Creates a summary of the conversation history.
        /// </summary>
        /// <param name="responses">The list of responses to summarize</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The summarized conversation history</returns>
        Task<string> SummarizeAsync(List<AiSceneResponse> responses, CancellationToken cancellationToken);

        /// <summary>
        /// Determines if the conversation history should be summarized.
        /// </summary>
        /// <param name="responses">The current list of responses</param>
        /// <returns>True if summarization is needed, false otherwise</returns>
        bool ShouldSummarize(List<AiSceneResponse> responses);
    }
}
