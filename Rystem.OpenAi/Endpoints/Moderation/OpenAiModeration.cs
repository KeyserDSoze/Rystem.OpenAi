using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Moderation
{
    internal sealed class OpenAiModeration : OpenAiBuilder<IOpenAiModeration, ModerationsRequest>, IOpenAiModeration
    {
        public OpenAiModeration(IFactory<DefaultServices> factory) : base(factory)
        {
        }
        /// <summary>
        /// Classifies if text violates OpenAI's Content Policy.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<ModerationResult> ExecuteAsync(string input, CancellationToken cancellationToken = default)
        {
            Request.Input = input;
            return DefaultServices.HttpClient.PostAsync<ModerationResult>(DefaultServices.Configuration.GetUri(OpenAiType.Moderation, Request.Model!, Forced, string.Empty), Request, DefaultServices.Configuration, cancellationToken);
        }
    }
}
