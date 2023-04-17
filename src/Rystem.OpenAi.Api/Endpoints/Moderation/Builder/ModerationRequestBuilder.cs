using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Moderation
{
    public sealed class ModerationRequestBuilder : RequestBuilder<ModerationsRequest>
    {
        private ModerationModelType _modelType;
        internal ModerationRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string input, IOpenAiUtility utility)
            : base(client, configuration, () =>
            {
                return new ModerationsRequest()
                {
                    Input = input,
                    ModelId = ModerationModelType.TextModerationLatest.ToModel().Id
                };
            }, utility)
        {
            _familyType = ModelFamilyType.Moderation;
            _modelType = ModerationModelType.TextModerationLatest;
        }
        /// <summary>
        /// Classifies if text violates OpenAI's Content Policy.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<ModerationResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => Client.PostAsync<ModerationResult>(Configuration.GetUri(OpenAiType.Moderation, Request.ModelId!, _forced), Request, Configuration, cancellationToken);
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Builder</returns>
        public ModerationRequestBuilder WithModel(ModerationModelType model)
        {
            Request.ModelId = model.ToModel().Id;
            _modelType = model;
            return this;
        }
        /// <summary>
        /// ID of the model to use. You can use <see cref="IOpenAiModelApi.ListAsync()"/> to see all of your available models, or use a standard model like <see cref="Model.TextModerationStable"/>.
        /// </summary>
        /// <param name="modelId">Override with a custom model id</param>
        /// <param name="basedOnFamily">Family of your custom model</param>
        /// <returns>Builder</returns>
        public ModerationRequestBuilder WithModel(string modelId, ModelFamilyType? basedOnFamily = null)
        {
            Request.ModelId = modelId;
            _forced = true;
            if (basedOnFamily != null)
                _familyType = basedOnFamily.Value;
            return this;
        }
        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns>decimal</returns>
        public decimal CalculateCost()
        {
            var tokenizer = Utility.Tokenizer.WithModerationModel(_modelType);
            var cost = Utility.Cost;
            var tokens = tokenizer.Encode(Request.Input).NumberOfTokens;
            return cost.Configure(settings =>
            {
                settings
                    .WithFamily(_familyType)
                    .WithType(OpenAiType.Moderation);
            }, Configuration.Name).Invoke(new OpenAiUsage
            {
                PromptTokens = tokens
            });
        }
    }
}
