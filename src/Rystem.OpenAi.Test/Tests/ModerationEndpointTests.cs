using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ModerationEndpointTests
    {
        private readonly IOpenAiFactory _openAiFactory;
        public ModerationEndpointTests(IOpenAiFactory openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        public async ValueTask CreateAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Moderation);

            var results = await openAiApi.Moderation
                .Create("I want to kill them and everyone else.")
                .WithModel(ModerationModelType.TextModerationStable)
                .ExecuteAsync();

            var categories = results.Results.First().Categories;
            Assert.NotNull(categories);
            Assert.True(categories.HateThreatening);

        }
    }
}
