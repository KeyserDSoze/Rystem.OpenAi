using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Moderation
{
    public interface IOpenAiModeration : IOpenAiBase<IOpenAiModeration, ModerationModelName>
    {
        /// <summary>
        /// Classifies if text violates OpenAI's Content Policy.
        /// </summary>
        /// <param name="input">
        /// The input text to classify.
        /// </param>
        /// <returns>Builder</returns>
        ValueTask<ModerationResult> ExecuteAsync(string input, CancellationToken cancellationToken = default);
    }
}
