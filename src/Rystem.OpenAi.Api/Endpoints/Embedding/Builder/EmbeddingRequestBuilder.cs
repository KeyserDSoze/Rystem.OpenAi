using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi;
using Rystem.OpenAi.Completion;
using Rystem.OpenAi.Edit;

namespace Rystem.OpenAi.Embedding
{
    public sealed class EmbeddingRequestBuilder : RequestBuilder<EmbeddingRequest>
    {
        private EmbeddingModelType _modelType;
        internal EmbeddingRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string[] inputs, IOpenAiUtility utility)
            : base(client, configuration, () =>
            {
                return new EmbeddingRequest()
                {
                    Input = inputs.Length == 1 ? inputs[0] : (object)inputs,
                    ModelId = EmbeddingModelType.AdaTextEmbedding.ToModel().Id,
                };
            }, utility)
        {
            _familyType = ModelFamilyType.Ada;
            _modelType = EmbeddingModelType.AdaTextEmbedding;
        }
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<EmbeddingResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => Client.PostAsync<EmbeddingResult>(Configuration.GetUri(OpenAiType.Embedding, Request.ModelId!, _forced, string.Empty), Request, Configuration, cancellationToken);
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>CostResult<EmbeddingResult></returns>
        public async ValueTask<CostResult<EmbeddingResult>> ExecuteAndCalculateCostAsync(CancellationToken cancellationToken = default)
        {
            var response = await ExecuteAsync(cancellationToken);
            return new CostResult<EmbeddingResult>(response, () => CalculateCost(OpenAiType.Embedding, response?.Usage));
        }
        /// <summary>
        /// Add further input to the request.
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Builder</returns>
        public EmbeddingRequestBuilder AddPrompt(string input)
        {
            if (Request.Input is string[] array)
            {
                var newArray = new string[array.Length + 1];
                array.CopyTo(newArray, 0);
                newArray[^1] = input;
                Request.Input = newArray;
            }
            else if (Request.Input is string value)
            {
                Request.Input = new string[2] { value, input };
            }
            else
            {
                Request.Input = input;
            }
            return this;
        }
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Builder</returns>
        public EmbeddingRequestBuilder WithModel(EmbeddingModelType model)
        {
            Request.ModelId = model.ToModel().Id;
            _familyType = model.ToFamily();
            _modelType = model;
            return this;
        }
        /// <summary>
        /// ID of the model to use. You can use <see cref="IOpenAiModelApi.ListAsync()"/> to see all of your available models, or use a standard model like <see cref="Model.DavinciText"/>.
        /// </summary>
        /// <param name="modelId">Override with a custom model id</param>
        /// <param name="basedOnFamily">Family of your custom model</param>
        /// <returns>Builder</returns>
        public EmbeddingRequestBuilder WithModel(string modelId, ModelFamilyType? basedOnFamily = null)
        {
            Request.ModelId = modelId;
            _forced = true;
            if (basedOnFamily != null)
                _familyType = basedOnFamily.Value;
            return this;
        }
        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids"></see>
        /// </summary>
        /// <param name="user">Unique identifier</param>
        /// <returns>Builder</returns>
        public EmbeddingRequestBuilder WithUser(string user)
        {
            Request.User = user;
            return this;
        }
        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns>decimal</returns>
        public decimal CalculateCost()
        {
            var tokenizer = Utility.Tokenizer.WithEmbeddingModel(_modelType);
            var cost = Utility.Cost;
            var tokens = 0;
            if (Request.Input is string[] array)
            {
                foreach (var x in array)
                    tokens += tokenizer.Encode(x).NumberOfTokens;
            }
            else if (Request.Input is string stringable)
            {
                tokens += tokenizer.Encode(stringable).NumberOfTokens;
            }
            return cost.Configure(settings =>
            {
                settings
                    .WithFamily(_familyType)
                    .WithType(OpenAiType.Embedding);
            }, Configuration.Name).Invoke(new OpenAiUsage
            {
                PromptTokens = tokens
            });
        }
    }
}
