using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class EmbeddingEndpointTests
    {
        private readonly IFactory<IOpenAi> _openAiFactory;
        private readonly IOpenAiUtility _openAiUtility;

        public EmbeddingEndpointTests(IFactory<IOpenAi> openAiFactory, IOpenAiUtility openAiUtility)
        {
            _openAiFactory = openAiFactory;
            _openAiUtility = openAiUtility;
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        [InlineData("NoDI")]
        public async ValueTask GetBasicEmbeddingAsync(string name)
        {
            var openAiApi = name == "NoDI" ? OpenAiServiceLocator.Instance.Create(name) : _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Embeddings);

            var results = await openAiApi.Embeddings
                .WithInputs("A test text for embedding")
                .ExecuteAsync();

            Assert.NotNull(results);
            if (results.CreatedUnixTime.HasValue)
            {
                Assert.True(results.CreatedUnixTime.Value != 0);
                Assert.NotNull(results.Created);
                Assert.True(results.Created.Value > new DateTime(2018, 1, 1));
                Assert.True(results.Created.Value < DateTime.Now.AddDays(1));
            }
            else
            {
                Assert.Null(results.Created);
            }
            Assert.NotNull(results.Object);
            Assert.True(results.Data.Count != 0);
            Assert.True(results.Data.First().Embedding.Length == 1536);
            var resultOfCosineSimilarity = _openAiUtility.CosineSimilarity(results.Data.First().Embedding, results.Data.First().Embedding);
            Assert.True(resultOfCosineSimilarity >= 0.9999d);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        [InlineData("NoDI")]
        public async ValueTask ReturnedUsageAsync(string integrationName)
        {
            var openAiApi = integrationName == "NoDI" ? OpenAiServiceLocator.Instance.Create(integrationName) : _openAiFactory.Create(integrationName);
            Assert.NotNull(openAiApi.Embeddings);

            var results = await openAiApi.Embeddings
                .WithInputs("A test text for embedding")
                .AddPrompt("with message")
                .WithUser("KeyserDSoze")
                .WithModel("testModel")
                .WithModel(EmbeddingModelName.Text_embedding_3_small)
                .ExecuteAsync();
            Assert.NotNull(results);

            Assert.NotNull(results.Usage);
            Assert.True(results.Usage.PromptTokens >= 5);
            Assert.True(results.Usage.TotalTokens >= results.Usage.PromptTokens);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        [InlineData("NoDI")]
        public async ValueTask GetBasicEmbeddingWithCustomDimensionsAsync(string name)
        {
            var openAiApi = name == "NoDI" ? OpenAiServiceLocator.Instance.Create(name) : _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Embeddings);

            var results = await openAiApi.Embeddings
                .AddPrompt("A test text for embedding")
                .WithModel("text-embedding-3-large")
                .WithDimensions(999)
                .ExecuteAsync();

            Assert.NotNull(results);
            if (results.CreatedUnixTime.HasValue)
            {
                Assert.True(results.CreatedUnixTime.Value != 0);
                Assert.NotNull(results.Created);
                Assert.True(results.Created.Value > new DateTime(2018, 1, 1));
                Assert.True(results.Created.Value < DateTime.Now.AddDays(1));
            }
            else
            {
                Assert.Null(results.Created);
            }
            Assert.NotNull(results.Object);
            Assert.True(results.Data.Count != 0);
            Assert.True(results.Data.First().Embedding.Length == 999);
            var resultOfCosineSimilarity = _openAiUtility.CosineSimilarity(results.Data.First().Embedding, results.Data.First().Embedding);
            Assert.True(resultOfCosineSimilarity >= 0.9999d);
        }
    }
}
