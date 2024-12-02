using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Embedding
{
    internal sealed class OpenAiEmbedding : OpenAiBuilder<IOpenAiEmbedding, EmbeddingRequest, EmbeddingModelName>, IOpenAiEmbedding
    {
        private readonly List<string> _inputs = [];
        public OpenAiEmbedding(IFactory<DefaultServices> factory) : base(factory)
        {
            Request.Model = EmbeddingModelName.Text_embedding_3_large;
        }
        public IOpenAiEmbedding WithInputs(params string[] inputs)
        {
            _inputs.AddRange(inputs);
            return this;
        }
        public IOpenAiEmbedding ClearInputs()
        {
            _inputs.Clear();
            return this;
        }
        public async ValueTask<EmbeddingResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Request.Input = _inputs;
            var response = await DefaultServices.HttpClient.PostAsync<EmbeddingResult>(DefaultServices.Configuration.GetUri(OpenAiType.Embedding, Request.Model!, Forced, string.Empty), Request, DefaultServices.Configuration, cancellationToken);
            if (response.Usage != null)
                Usages.Add(new OpenAiCost { Units = response.Usage.TotalTokens, UnitOfMeasure = UnitOfMeasure.Tokens, Kind = KindOfCost.Input });
            return response;
        }
        public IOpenAiEmbedding AddPrompt(string input)
        {
            _inputs.Add(input);
            return this;
        }
        public IOpenAiEmbedding WithUser(string user)
        {
            Request.User = user;
            return this;
        }
        public IOpenAiEmbedding WithDimensions(int dimensions)
        {
            Request.Dimensions = dimensions;
            return this;
        }
        private const string Base64Label = "base64";
        private const string FloatLabel = "float";
        public IOpenAiEmbedding WithEncodingFormat(EncodingFormatForEmbedding encodingFormat)
        {
            //todo: check encoding format base65 what kind of output we have
            Request.EncodingFormat = encodingFormat switch
            {
                EncodingFormatForEmbedding.Base64 => Base64Label,
                _ => FloatLabel,
            };
            return this;
        }
    }
}
