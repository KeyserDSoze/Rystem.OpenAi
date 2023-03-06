using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.Models;

namespace Rystem.OpenAi.Embedding
{
    public sealed class EmbeddingRequestBuilder : RequestBuilder<EmbeddingRequest>
    {
        internal EmbeddingRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string[] inputs)
            : base(client, configuration, () =>
            {
                return new EmbeddingRequest()
                {
                    Input = inputs.Length == 1 ? inputs[0] : (object)inputs,
                    ModelId = EmbeddingModelType.AdaTextEmbedding.ToModel().Id,
                };
            })
        {
        }
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<EmbeddingResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => _client.PostAsync<EmbeddingResult>(_configuration.GetUri(OpenAi.Embedding,_request.ModelId!), _request, cancellationToken);
        /// <summary>
        /// Add further input to the request.
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Builder</returns>
        public EmbeddingRequestBuilder AddPrompt(string input)
        {
            if (_request.Input is string[] array)
            {
                var newArray = new string[array.Length + 1];
                array.CopyTo(newArray, 0);
                newArray[^1] = input;
                _request.Input = newArray;
            }
            else if (_request.Input is string value)
            {
                _request.Input = new string[2] { value, input };
            }
            else
            {
                _request.Input = input;
            }
            return this;
        }
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public EmbeddingRequestBuilder WithModel(EmbeddingModelType model)
        {
            _request.ModelId = model.ToModel().Id;
            return this;
        }
        /// <summary>
        /// ID of the model to use. You can use <see cref="IOpenAiModelApi.AllAsync()"/> to see all of your available models, or use a standard model like <see cref="Model.DavinciText"/>.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public EmbeddingRequestBuilder WithModel(string modelId)
        {
            _request.ModelId = modelId;
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
            _request.User = user;
            return this;
        }
    }
}
