using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rystem.OpenAi.Completion;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class CompletionEndpointTests
    {
        private readonly IOpenAiApi _openAiApi;
        public CompletionEndpointTests(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        [Fact]
        public async ValueTask GetBasicCompletionAsync()
        {
            Assert.NotNull(_openAiApi.Completion);

            var results = await _openAiApi.Completion
                .Request("One Two Three Four Five Six Seven Eight Nine One Two Three Four Five Six Seven Eight")
                .WithModel(TextModelType.CurieText)
                .WithTemperature(0.1)
                .SetMaxTokens(5)
                .ExecuteAsync();

            Assert.NotNull(results);
            Assert.NotNull(results.CreatedUnixTime);
            Assert.True(results.CreatedUnixTime.Value != 0);
            Assert.NotNull(results.Created);
            Assert.NotNull(results.Completions);
            Assert.True(results.Completions.Count != 0);
            Assert.Contains(results.Completions, c => c.Text.Trim().ToLower().StartsWith("nine"));
        }


        [Fact]
        public async ValueTask GetSimpleCompletionAsync()
        {
            Assert.NotNull(_openAiApi.Completion);

            var results = await _openAiApi.Completion
                .Request("One Two Three Four Five Six Seven Eight Nine One Two Three Four Five Six Seven Eight")
                .WithTemperature(0.1)
                .SetMaxTokens(5)
                .ExecuteAsync();
            Assert.NotNull(results);
            Assert.True(results.Completions.Count > 0);
            Assert.Contains(results.Completions, c => c.Text.Trim().ToLower().StartsWith("nine"));
        }


        [Fact]
        public async ValueTask CompletionUsageDataWorksAsync()
        {
            Assert.NotNull(_openAiApi.Completion);

            var results = await _openAiApi.Completion
               .Request("One Two Three Four Five Six Seven Eight Nine One Two Three Four Five Six Seven Eight")
               .WithModel(TextModelType.CurieText)
               .WithTemperature(0.1)
               .SetMaxTokens(5)
               .ExecuteAsync();
            Assert.NotNull(results);
            Assert.NotNull(results.Usage);
            Assert.True(results.Usage.PromptTokens > 15);
            Assert.True(results.Usage.CompletionTokens > 0);
            Assert.True(results.Usage.TotalTokens >= results.Usage.PromptTokens + results.Usage.CompletionTokens);
        }


        [Fact]
        public async Task CreateCompletionAsync_MultiplePrompts_ShouldReturnResult()
        {
            Assert.NotNull(_openAiApi.Completion);

            var results = new List<CompletionResult>();
            await foreach (var x in _openAiApi.Completion
               .Request("Today is Monday, tomorrow is", "10 11 12 13 14")
               .WithTemperature(0)
               .SetMaxTokens(3)
               .ExecuteAsStreamAsync())
            {
                results.Add(x);
            }

            Assert.NotEmpty(results);
            Assert.Contains(results.First().Completions, c => c.Text.Trim().ToLower().Contains("tuesday"));
            Assert.Contains(results.SelectMany(x => x.Completions), c => c.Text.Trim().ToLower().Contains("15"));
        }
    }
}
