using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rystem.OpenAi.Completion;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class CompletionEndpointTests
    {
        private readonly IOpenAiFactory _openAiFactory;
        public CompletionEndpointTests(IOpenAiFactory openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask GetBasicCompletionAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Completion);

            var results = await openAiApi.Completion
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
            Assert.Contains(results.Completions.Select(x => x.Logprobs), x => x == null);
            Assert.Contains(results.Completions, c => c.Text.Trim().ToLower().StartsWith("nine"));
        }


        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask GetSimpleCompletionAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Completion);

            var results = await openAiApi.Completion
                .Request("One Two Three Four Five Six Seven Eight Nine One Two Three Four Five Six Seven Eight")
                .WithTemperature(0.1)
                .SetMaxTokens(5)
                .ExecuteAsync();
            Assert.NotNull(results);
            Assert.True(results.Completions.Count > 0);
            Assert.Contains(results.Completions, c => c.Text.Trim().ToLower().StartsWith("nine"));
        }


        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CompletionUsageDataWorksAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Completion);
            var biasDictionary = new Dictionary<string, int>
            {
                { "Keystone", 1 },
                { "Keystone3", 4 }
            };
            var response = await openAiApi.Completion
               .Request("One Two Three Four Five Six Seven Eight Nine One Two Three")
               .WithModel(TextModelType.CurieText)
               .WithTemperature(0.1)
               .AddPrompt("")
               .AddPrompt("Four Five Six Seven Eight")
               .WithSuffix("ale")
               .WithNucleusSampling(1)
               .WithNumberOfChoicesPerPrompt(1)
               .WithLogProbs(1)
               .WithEcho()
               .WithStopSequence("alessandro")
               .AddStopSequence("alessandro2")
               .WithFrequencyPenalty(0)
               .WithPresencePenalty(0)
               .BestOf(1)
               .WithBias("Keystone", 4)
               .WithBias("Keystone2", 4)
               .WithBias(biasDictionary)
               .WithUser("KeyserDSoze")
               .SetMaxTokens(5)
               .ExecuteAndCalculateCostAsync();
            var results = response.Result;
            Assert.NotNull(results);
            Assert.NotNull(results.Usage);
            Assert.True(results.Usage.PromptTokens > 15);
            Assert.True(results.Usage.CompletionTokens > 0);
            Assert.True(results.Usage.TotalTokens >= results.Usage.PromptTokens + results.Usage.CompletionTokens);
        }


        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async Task CreateCompletionAsync_MultiplePrompts_ShouldReturnResult(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Completion);

            var results = new List<CompletionResult>();
            await foreach (var x in openAiApi.Completion
               .Request("Today is Monday, tomorrow is", "10 11 12 13 14")
               .WithTemperature(0)
               .SetMaxTokens(3)
               .ExecuteAsStreamAsync())
            {
                results.Add(x);
            }

            Assert.NotEmpty(results);
            Assert.Contains(results.SelectMany(x => x.Completions), c => c.Text.Trim().ToLower().Contains("tuesday"));
            Assert.Contains(results.SelectMany(x => x.Completions), c => c.Text.Trim().ToLower().Contains("15"));
        }
    }
}
