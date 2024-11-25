using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi.Moderation
{
    internal sealed class OpenAiModeration : OpenAiBuilder<ModerationsRequest>, IOpenAiModeration
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
            return DefaultServices.HttpClient.PostAsync<ModerationResult>(Configuration.GetUri(OpenAiType.Moderation, Request.Model!, Forced, string.Empty), Request, DefaultServices.Configuration, cancellationToken);
        }
    }
}
