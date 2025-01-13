using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ModerationEndpointTests
    {
        private readonly IFactory<IOpenAi> _openAiFactory;

        public ModerationEndpointTests(IFactory<IOpenAi> openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Moderation);

            var results = await openAiApi.Moderation
                .ForceModel("testModel")
                .WithModel(ModerationModelName.OmniLatest)
                .ExecuteAsync("I want to kill them and everyone else.");

            var categories = results.Results?.FirstOrDefault()?.Categories;
            Assert.NotNull(categories);
            Assert.True(categories.HarassmentThreatening);
        }
    }
}
