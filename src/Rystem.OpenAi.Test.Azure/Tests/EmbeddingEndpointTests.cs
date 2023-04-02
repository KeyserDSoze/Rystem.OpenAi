using System;
using System.Linq;
using System.Threading.Tasks;
using Rystem.OpenAi;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class EmbeddingEndpointTests
    {
        private readonly IOpenAiApi _openAiApi;
        private readonly IOpenAiUtility _openAiUtility;

        public EmbeddingEndpointTests(IOpenAiApi openAiApi, IOpenAiUtility openAiUtility)
        {
            _openAiApi = openAiApi;
            _openAiUtility = openAiUtility;
        }
        [Fact]
        public async ValueTask GetBasicEmbeddingAsync()
        {
            Assert.NotNull(_openAiApi.Embedding);

            var results = await _openAiApi.Embedding
                .Request("A test text for embedding")
                .WithModel("Embedding")
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
            Assert.True(resultOfCosineSimilarity >= 1);
        }

        [Fact]
        public async ValueTask ReturnedUsageAsync()
        {
            Assert.NotNull(_openAiApi.Embedding);

            var results = await _openAiApi.Embedding.Request("A test text for embedding")
                .WithModel("Embedding")
                .ExecuteAsync();
            Assert.NotNull(results);

            Assert.NotNull(results.Usage);
            Assert.True(results.Usage.PromptTokens >= 5);
            Assert.True(results.Usage.TotalTokens >= results.Usage.PromptTokens);
        }
    }
}
