namespace Rystem.OpenAi.Moderation
{
    public interface IOpenAiModerationApi
    {
        /// <summary>
        /// Classifies if text violates OpenAI's Content Policy.
        /// </summary>
        /// <param name="input">
        /// The input text to classify.
        /// </param>
        /// <returns>Builder</returns>
        ModerationRequestBuilder Create(string input);
    }
}
