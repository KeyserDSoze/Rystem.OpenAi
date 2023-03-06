using System;
using System.Threading.Tasks;
using Rystem.OpenAi;
using Rystem.OpenAi.Models;
using Xunit;

namespace Azure.OpenAi.Test
{
    public class ModelEndpointTests
    {
        private readonly IOpenAiApi _openAiApi;
        public ModelEndpointTests(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        [Fact]
        public async ValueTask GetAllModelsAsync()
        {
            Assert.NotNull(_openAiApi.Model);
            var results = await _openAiApi.Model.ListAsync();
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Contains(results, c => c.Id.ToLower().StartsWith("text-davinci"));
        }

        [Fact]
        public async ValueTask GetModelDetailsAsync()
        {
            Assert.NotNull(_openAiApi.Model);

            var result = await _openAiApi.Model.RetrieveAsync(TextModelType.DavinciText3.ToModel().Id);
            Assert.NotNull(result);

            Assert.NotNull(result.CreatedUnixTime);
            Assert.True(result.CreatedUnixTime.Value != 0);
            Assert.NotNull(result.Created);
            Assert.True(result.Created.Value > new DateTime(2018, 1, 1));
            Assert.True(result.Created.Value < DateTime.Now.AddDays(1));

            Assert.NotNull(result.Id);
            Assert.NotNull(result.OwnedBy);
            Assert.Equal(TextModelType.DavinciText3.ToModel().Id, result.Id.ToLower());
        }


        [Fact]
        public async ValueTask GetEnginesAsync_ShouldReturnTheEngineList()
        {
            var models = await _openAiApi.Model.ListAsync();
            Assert.True(models.Count > 5);
        }

        [Theory]
        [InlineData("ada")]
        [InlineData("babbage")]
        [InlineData("curie")]
        [InlineData("davinci")]
        public async ValueTask RetrieveEngineDetailsAsync_ShouldRetrieveEngineDetails(string modelId)
        {
            var modelData = await _openAiApi.Model.RetrieveAsync(modelId);
            Assert.Equal(modelId, modelData.Id);
            Assert.True(modelData.Created > new DateTime(2018, 1, 1));
            Assert.True(modelData.Created < DateTime.UtcNow.AddDays(1));
        }
    }
}
