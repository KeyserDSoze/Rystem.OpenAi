using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rystem.OpenAi.Completion;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ModerationEndpointTests
    {
        private readonly IOpenAiApi _openAiApi;
        public ModerationEndpointTests(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        [Fact]
        public async ValueTask CreateAsync()
        {
            Assert.NotNull(_openAiApi.Moderation);

            var results = await _openAiApi.Moderation
                .Create("I want to kill them.")
                .WithModel(ModerationModelType.TextModerationStable)
                .ExecuteAsync();

            var categories = results.Results.First().Categories;
            Assert.NotNull(categories);
            Assert.True(categories.HateThreatening);

        }
    }
}
