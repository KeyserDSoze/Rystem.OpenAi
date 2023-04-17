using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
        [InlineData(ModelFamilyType.Gpt3_5, OpenAiType.Chat, "", 1, "This model is the perfect model for you.", 9, 0.002, 3, 5, "Gpt35Turbo")]
        [InlineData(ModelFamilyType.Gpt3_5, OpenAiType.Chat, "", 3, "Multiple models, each with different capabilities and price points. Prices are per 1000 tokens. You can think of tokens as pieces of words, where 1000 tokens is about 750 words.", 40, 0.002, 3, 5, "Gpt35Turbo")]
        [InlineData(ModelFamilyType.Gpt3_5, OpenAiType.Chat, "", 7, "Multiple models, each with different capabilities and price points. Prices are per 1000 tokens. You can think of tokens as pieces of words, where 1000 tokens is about 750 words.", 40, 0.002, 3, 5, "Gpt35Turbo")]
        public async Task Gpt3_5(ModelFamilyType model, OpenAiType type, string name, int times, string content, int numberOfTokens, decimal currentPrice, int startingAndEndingTokens, int fixedTokens, string modelType)
        {
            var promptedTokens = times * numberOfTokens + (times * fixedTokens) + startingAndEndingTokens;
            var openAiApi = _openAiFactory.Create(name);
            var manualCostCalculator = _openAiCost.Configure(configuration =>
            {
                configuration
                    .WithFamily(model)
                    .WithType(type);
            });
            var manualCalculatedPrice = manualCostCalculator.Invoke(new OpenAiUsage
            {
                PromptTokens = promptedTokens,
            });
            var responseWithCost = await ExecuteRequestWithCostAsync(openAiApi, type, content, times, modelType);
            var price = responseWithCost.CostCalculatedBySystem.Invoke();
            Assert.Equal(promptedTokens * currentPrice / 1_000, price);
            Assert.Equal(manualCalculatedPrice, price);
            var finalPriceForEntireOperation = responseWithCost.CostCalculation.Invoke();
            var tokenizer = responseWithCost.Tokenizer;
            var tokenForResponse = 0;
            foreach (var message in responseWithCost.Contents)
                tokenForResponse += tokenizer.Encode(message).NumberOfTokens;
            var manualFinalCalculatedPrice = manualCostCalculator.Invoke(new OpenAiUsage
            {
                PromptTokens = promptedTokens,
                CompletionTokens = tokenForResponse
            });
            Assert.True(finalPriceForEntireOperation > price);
            Assert.Equal(finalPriceForEntireOperation, manualFinalCalculatedPrice);
        }
        private async Task<(Func<decimal> CostCalculatedBySystem, Func<decimal> CostCalculation, List<string> Contents, int PromptTokens, int CompletionTokes, IOpenAiTokenizer Tokenizer)> ExecuteRequestWithCostAsync(IOpenAiApi openAiApi, OpenAiType type, string content, int times, string modelType)
        {
            switch (type)
            {
                case OpenAiType.Chat:
                    var model = (ChatModelType)Enum.Parse(typeof(ChatModelType), modelType);
                    var chat = openAiApi.Chat
                     .Request(new ChatMessage { Role = ChatRole.User, Content = content })
                     .WithModel(model)
                     .WithTemperature(1);
                    if (times > 1)
                        for (var i = 1; i < times; i++)
                            chat.AddMessage(new ChatMessage { Role = ChatRole.User, Content = content });
                    var responseWithCost = await chat.ExecuteAndCalculateCostAsync();
                    var tokenizer = _utility.Tokenizer.WithChatModel(model);
                    return (() => chat.CalculateCost(), () => responseWithCost.CalculateCost(), responseWithCost.Result.Choices.Select(x => x.Message.Content).Where(x => x != null).ToList(), responseWithCost.Result.Usage.PromptTokens.Value, responseWithCost.Result.Usage.CompletionTokens.Value, tokenizer);
            }
            return default;
        }
    }
}
