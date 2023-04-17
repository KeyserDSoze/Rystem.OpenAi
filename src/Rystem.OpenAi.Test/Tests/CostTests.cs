using System;
using System.Collections.Generic;
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
        [InlineData(ModelFamilyType.Gpt3_5, OpenAiType.Chat, "", 1, "This model is the perfect model for you.", 9, 0.002, 3, 5, "Gpt35Turbo", false)]
        [InlineData(ModelFamilyType.Gpt3_5, OpenAiType.Chat, "", 3, "Multiple models, each with different capabilities and price points. Prices are per 1000 tokens. You can think of tokens as pieces of words, where 1000 tokens is about 750 words.", 40, 0.002, 3, 5, "Gpt35Turbo", false)]
        [InlineData(ModelFamilyType.Gpt3_5, OpenAiType.Chat, "", 7, "Multiple models, each with different capabilities and price points. Prices are per 1000 tokens. You can think of tokens as pieces of words, where 1000 tokens is about 750 words.", 40, 0.002, 3, 5, "Gpt35Turbo", false)]
        [InlineData(ModelFamilyType.Davinci, OpenAiType.Completion, "", 1, "one two three four five six seven", 7, 0.02, 0, 0, "DavinciText3", false)]
        [InlineData(ModelFamilyType.Davinci, OpenAiType.Edit, "", 2, "Fix the spelling mistakes", 4, 0.02, 10, 1, "TextDavinciEdit", true)]
        [InlineData(ModelFamilyType.Davinci, OpenAiType.Edit, "", 1, "Fix the spelling mistakes", 4, 0.02, 10, 1, "TextDavinciEdit", true)]
        public async Task CalculateCosts(ModelFamilyType model, OpenAiType type, string name, int times, string content, int numberOfTokens, decimal currentPrice, int startingAndEndingTokens, int fixedTokens, string modelType, bool imNotAbleToUnderstandHowToCalculateCompletionTokens)
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
            if (!imNotAbleToUnderstandHowToCalculateCompletionTokens)
                Assert.Equal(tokenForResponse, responseWithCost.CompletionTokens);
            var manualFinalCalculatedPrice = manualCostCalculator.Invoke(new OpenAiUsage
            {
                PromptTokens = promptedTokens,
                CompletionTokens = tokenForResponse
            });
            Assert.True(finalPriceForEntireOperation > price);
            if (!imNotAbleToUnderstandHowToCalculateCompletionTokens)
                Assert.Equal(finalPriceForEntireOperation, manualFinalCalculatedPrice);
        }
        //todo understand the calculation for completionTokens in Edit endpoint
        private async Task<(Func<decimal> CostCalculatedBySystem, Func<decimal> CostCalculation, List<string> Contents, int PromptTokens, int CompletionTokens, IOpenAiTokenizer Tokenizer)> ExecuteRequestWithCostAsync(IOpenAiApi openAiApi, OpenAiType type, string content, int times, string modelType)
        {
            switch (type)
            {
                case OpenAiType.Chat:
                    var chatModel = (ChatModelType)Enum.Parse(typeof(ChatModelType), modelType);
                    var chat = openAiApi.Chat
                     .Request(new ChatMessage { Role = ChatRole.User, Content = content })
                     .WithModel(chatModel)
                     .WithTemperature(1);
                    if (times > 1)
                        for (var i = 1; i < times; i++)
                            chat.AddMessage(new ChatMessage { Role = ChatRole.User, Content = content });
                    var responseForChatWithCost = await chat.ExecuteAndCalculateCostAsync();
                    var chatTokenizer = _utility.Tokenizer.WithChatModel(chatModel);
                    return (() => chat.CalculateCost(),
                            () => responseForChatWithCost.CalculateCost(),
                            responseForChatWithCost.Result.Choices.Select(x => x.Message.Content).Where(x => x != null).ToList(),
                            responseForChatWithCost.Result.Usage.PromptTokens.Value,
                            responseForChatWithCost.Result.Usage.CompletionTokens.Value,
                            chatTokenizer);
                case OpenAiType.Completion:
                    var modelForCompletion = (TextModelType)Enum.Parse(typeof(TextModelType), modelType);
                    var completion = openAiApi.Completion
                     .Request(content)
                     .WithModel(modelForCompletion)
                     .WithTemperature(1);
                    if (times > 1)
                        for (var i = 1; i < times; i++)
                            completion.AddPrompt(content);
                    var responseForCompletionWithCost = await completion.ExecuteAndCalculateCostAsync();
                    var completionTokenizer = _utility.Tokenizer.WithTextModel(modelForCompletion);
                    return (() => completion.CalculateCost(),
                            () => responseForCompletionWithCost.CalculateCost(),
                            responseForCompletionWithCost.Result.Completions.Select(x => x.Text).Where(x => x != null).ToList(),
                            responseForCompletionWithCost.Result.Usage.PromptTokens.Value,
                            responseForCompletionWithCost.Result.Usage.CompletionTokens.Value,
                            completionTokenizer);
                case OpenAiType.Edit:
                    var modelForEdit = (EditModelType)Enum.Parse(typeof(EditModelType), modelType);
                    var edit = openAiApi.Edit
                         .Request(content)
                         .WithModel(modelForEdit)
                         .WithTemperature(1);
                    if (times > 1)
                        for (var i = 1; i < times; i++)
                            edit.SetInput(content);
                    var responseForEditWithCost = await edit.ExecuteAndCalculateCostAsync();
                    var editTokenizer = _utility.Tokenizer.WithEditModel(modelForEdit);
                    return (() => edit.CalculateCost(),
                            () => responseForEditWithCost.CalculateCost(),
                            responseForEditWithCost.Result.Choices.Select(x => x.Text).Where(x => x != null).ToList(),
                            responseForEditWithCost.Result.Usage.PromptTokens.Value,
                            responseForEditWithCost.Result.Usage.CompletionTokens.Value,
                            editTokenizer);
            }
            return default;
        }
    }
}
