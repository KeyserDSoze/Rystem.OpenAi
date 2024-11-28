using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ModerationEndpointTests
    {
        private readonly IFactory<IOpenAi> _openAiFactory;
        private readonly IOpenAiUtility _openAiUtility;

        public ModerationEndpointTests(IFactory<IOpenAi> openAiFactory, IOpenAiUtility openAiUtility)
        {
            _openAiFactory = openAiFactory;
            _openAiUtility = openAiUtility;
        }
        [Theory]
        [InlineData("")]
        public async ValueTask CreateAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Moderation);

            var results = await openAiApi.Moderation
                .WithModel("testModel")
                .WithModel(ModerationModelName.OmniLatest)
                .ExecuteAsync("I want to kill them and everyone else.");

            var categories = results.Results.First().Categories;
            Assert.NotNull(categories);
            Assert.True(categories.HateThreatening);

        }
    }
}
