namespace Rystem.OpenAi.Assistant
{
    public interface IOpenAiFileSearchAssistant<T>
    {
        /// <summary>
        /// The ranking options for the file search. If not specified, the file search tool will use the auto ranker and a score_threshold of 0.
        /// </summary>
        /// <returns></returns>
        T UseDefault();
        /// <summary>
        /// The ranking options for the file search. If not specified, the file search tool will use the auto ranker and a score_threshold of 0.
        /// </summary>
        /// <param name="ranker">The ranker to use for the file search. If not specified will use the auto ranker. Which ranker to use in determining which chunks to use. The available values are auto, which uses the latest available ranker, and default_2024_08_21.</param>
        /// <param name="scoreThreshold">The score threshold for the file search. All values must be a floating point number between 0 and 1.  a ranking between 0.0 and 1.0, with 1.0 being the highest ranking. A higher number will constrain the file chunks used to generate a result to only chunks with a higher possible relevance, at the cost of potentially leaving out relevant chunks.</param>
        /// <returns></returns>
        T UseCustom(RankerName ranker, int scoreThreshold);
    }
}
