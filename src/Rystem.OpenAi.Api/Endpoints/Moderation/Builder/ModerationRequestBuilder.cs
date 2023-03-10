using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi;

namespace Rystem.OpenAi.Moderation
{
    public sealed class ModerationRequestBuilder : RequestBuilder<ModerationsRequest>
    {
        internal ModerationRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string input)
            : base(client, configuration, () =>
            {
                return new ModerationsRequest()
                {
                    Input = input,
                    ModelId = ModerationModelType.TextModerationLatest.ToModel().Id
                };
            })
        {
        }
        /// <summary>
        /// Classifies if text violates OpenAI's Content Policy.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<ModerationsResponse> ExecuteAsync(CancellationToken cancellationToken = default)
            => _client.PostAsync<ModerationsResponse>(_configuration.GetUri(OpenAiType.Moderation, _request.ModelId!, _forced), _request, _configuration, cancellationToken);
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public ModerationRequestBuilder WithModel(ModerationModelType model)
        {
            _request.ModelId = model.ToModel().Id;
            return this;
        }
        /// <summary>
        /// ID of the model to use. You can use <see cref="IOpenAiModelApi.AllAsync()"/> to see all of your available models, or use a standard model like <see cref="Model.TextModerationStable"/>.
        /// </summary>
        /// <param name="modelId">Override with a custom model id</param>
        /// <returns>Builder</returns>
        public ModerationRequestBuilder WithModel(string modelId)
        {
            _request.ModelId = modelId;
            _forced = true;
            return this;
        }
    }
}
