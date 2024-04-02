using System;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ModelEndpointTests
    {
        private readonly IOpenAiFactory _openAiFactory;
        public ModelEndpointTests(IOpenAiFactory openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask GetAllModelsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            await openAiApi.Model.DeleteAsync("dadsadasdasdad");
            Assert.NotNull(openAiApi.Model);
            var results = await openAiApi.Model.ListAsync();
            foreach (var model in results.Items)
            {
                var modelResult = await openAiApi.Model.RetrieveAsync(model.Id);
                Assert.NotNull(modelResult);
                Assert.Equal(model.Id, modelResult.Id);
                if (string.IsNullOrWhiteSpace(name))
                    Assert.True(modelResult.Created > new DateTime(2018, 1, 1));
            }
            Assert.NotNull(results);
            Assert.NotEmpty(results.Items);
            Assert.Contains(results.Items, c => c.Id.ToLower().StartsWith("text-davinci"));
        }

        [Theory]
        [InlineData("")]
        public async ValueTask GetModelDetailsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Model);

            var result = await openAiApi.Model.RetrieveAsync(TextModelType.DavinciText3.ToModel());
            Assert.NotNull(result);

            Assert.NotNull(result.CreatedUnixTime);
            Assert.True(result.CreatedUnixTime.Value != 0);
            Assert.NotNull(result.Created);
            Assert.True(result.Created.Value > new DateTime(2018, 1, 1));
            Assert.True(result.Created.Value < DateTime.Now.AddDays(1));

            Assert.NotNull(result.Id);
            Assert.NotNull(result.OwnedBy);
            Assert.Equal(TextModelType.DavinciText3.ToModel(), result.Id.ToLower());
        }


        [Theory]
        [InlineData("")]
        public async ValueTask GetEnginesAsync_ShouldReturnTheEngineList(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            var models = await openAiApi.Model.ListAsync();
            Assert.True(models.Items.Count > 5);
        }

        [Theory]
        [InlineData("ada", "")]
        [InlineData("babbage", "")]
        [InlineData("curie", "")]
        [InlineData("davinci", "")]
        public async ValueTask RetrieveEngineDetailsAsync_ShouldRetrieveEngineDetails(string modelId, string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            var modelData = await openAiApi.Model.RetrieveAsync(modelId);
            Assert.Equal(modelId, modelData.Id);
            Assert.True(modelData.Created > new DateTime(2018, 1, 1));
            Assert.True(modelData.Created < DateTime.UtcNow.AddDays(1));
        }
    }
}
