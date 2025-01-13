using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Moderation
{
    internal sealed class OpenAiModeration : OpenAiBuilder<IOpenAiModeration, ModerationsRequest, ModerationModelName>, IOpenAiModeration
    {
        public OpenAiModeration(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLogger logger)
            : base(factory, configurationFactory, logger, OpenAiType.Moderation)
        {
            Request.Model = ModerationModelName.OmniLatest;
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.Moderation != null)
            {
                configuration.Settings.DefaultRequestConfiguration.Moderation.Invoke(this);
            }
        }
        /// <summary>
        /// Classifies if text violates OpenAI's Content Policy.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<ModerationResult> ExecuteAsync(string input, CancellationToken cancellationToken = default)
        {
            Request.Input = input;
            return DefaultServices.HttpClientWrapper
                    .PostAsync<ModerationResult>(
                        DefaultServices.Configuration.GetUri(OpenAiType.Moderation, _version, Request.Model!, string.Empty, null),
                        Request,
                        null,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }
    }
}
