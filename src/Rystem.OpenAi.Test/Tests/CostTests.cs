using System.Linq;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class CostTests
    {
        private readonly IOpenAiFactory _openAiFactory;
        private readonly IOpenAiCost _openAiCost;
        private readonly IOpenAiUtility _utility;

        public CostTests(IOpenAiFactory openAiFactory, IOpenAiCost openAiCost, IOpenAiUtility utility)
        {
            _openAiFactory = openAiFactory;
            _openAiCost = openAiCost;
            _utility = utility;
        }
        [Theory]
        [InlineData(9, 0.002, 3)]
        public void CalculateManually(int numberOfTokens, decimal currentPrice, int times)
        {
            var manualCostCalculator = _openAiCost.Configure(x =>
            {
                x
                .WithFamily(ModelFamilyType.Gpt3_5)
                .WithType(OpenAiType.Chat);
            });
            var manualCalculatedPrice = manualCostCalculator.Invoke(new OpenAiUsage
            {
                PromptTokens = numberOfTokens * times,
            });
            Assert.Equal(times * numberOfTokens * currentPrice / 1_000, manualCalculatedPrice);
        }
        [Theory]
        [InlineData("", 1, "This model is the perfect model for you.", 9, 0.002, 3, 5)]
        [InlineData("", 3, "Multiple models, each with different capabilities and price points. Prices are per 1000 tokens. You can think of tokens as pieces of words, where 1000 tokens is about 750 words.", 40, 0.002, 3, 5)]
        [InlineData("", 7, "Multiple models, each with different capabilities and price points. Prices are per 1000 tokens. You can think of tokens as pieces of words, where 1000 tokens is about 750 words.", 40, 0.002, 3, 5)]
        public async Task Gpt3_5(string name, int times, string value, int numberOfTokens, decimal currentPrice, int fixedTokens, int startingAndEndingTokens)
        {
            var promptedTokens = times * numberOfTokens + (times * fixedTokens) + startingAndEndingTokens;
            var openAiApi = _openAiFactory.Create(name);
            var manualCostCalculator = _openAiCost.Configure(configuration =>
            {
                configuration
                    .WithFamily(ModelFamilyType.Gpt3_5)
                    .WithType(OpenAiType.Chat);
            });
            var manualCalculatedPrice = manualCostCalculator.Invoke(new OpenAiUsage
            {
                PromptTokens = promptedTokens,
            });
            Assert.NotNull(openAiApi.Chat);
            var chat = openAiApi.Chat
                .Request(new ChatMessage { Role = ChatRole.User, Content = value })
                .WithModel(ChatModelType.Gpt35Turbo0301)
                .WithTemperature(1);
            if (times > 1)
                for (var i = 1; i < times; i++)
                    chat.AddMessage(new ChatMessage { Role = ChatRole.User, Content = value });
            var price = chat
                .CalculateCost();
            Assert.Equal(promptedTokens * currentPrice / 1_000, price);
            Assert.Equal(manualCalculatedPrice, price);
            var responseWithCost = await chat.ExecuteAndCalculateCostAsync();
            var finalPriceForEntireOperation = responseWithCost.CalculateCost();
            var tokenizer = _utility.Tokenizer.WithChatModel(ChatModelType.Gpt35Turbo);
            var tokenForResponse = 0;
            foreach (var content in responseWithCost.Result.Choices.Select(x => x.Message.Content))
                tokenForResponse += tokenizer.Encode(content).NumberOfTokens;
            var manualFinalCalculatedPrice = manualCostCalculator.Invoke(new OpenAiUsage
            {
                PromptTokens = numberOfTokens * times,
                CompletionTokens = tokenForResponse
            });
            Assert.True(finalPriceForEntireOperation > price);
        }
    }
}
