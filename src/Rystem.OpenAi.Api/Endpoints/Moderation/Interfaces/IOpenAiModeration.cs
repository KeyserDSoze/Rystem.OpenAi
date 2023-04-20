using System;

namespace Rystem.OpenAi.Moderation
{
    public interface IOpenAiModeration
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
    [Obsolete("In version 3.x we'll remove IOpenAiModerationApi and we'll use only IOpenAiModeration to retrieve services")]
    public interface IOpenAiModerationApi : IOpenAiModeration
    {
    }
}
