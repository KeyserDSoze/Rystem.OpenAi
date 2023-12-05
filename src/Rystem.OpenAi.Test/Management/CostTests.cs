using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Image;
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
        [InlineData("", 9, 0.002, 3)]
        public void CalculateManually(string integrationName, int numberOfTokens, decimal currentPrice, int times)
        {
            var manualCostCalculator = _openAiCost.Configure(x =>
            {
                x
                .WithFamily(ModelFamilyType.Gpt3_5)
                .WithType(OpenAiType.Chat);
            }, integrationName);
            var manualCalculatedPrice = manualCostCalculator.Invoke(new OpenAiUsage
            {
                PromptTokens = numberOfTokens * times,
            });
            Assert.Equal(times * numberOfTokens * currentPrice / 1_000, manualCalculatedPrice);
        }
        [Theory]
        [InlineData("", 0)]
        public void CalculateModeration(string integrationName, decimal currentPrice)
        {
            var openAiApi = _openAiFactory.Create(integrationName);
            var price = openAiApi.Moderation
                .Create("i want to kill everyone.")
                .WithModel("moderationTestModel", ModelFamilyType.Moderation)
                .WithModel(ModerationModelType.TextModerationLatest)
                .CalculateCost();
            Assert.Equal(currentPrice, price);
        }
        [Theory]
        [InlineData("", ModelFamilyType.Ada, true, 40, 0.0004)]
        [InlineData("", ModelFamilyType.Ada, false, 40, 0.0016)]
        [InlineData("Azure", ModelFamilyType.Ada, false, 40, 0.0016)]
        public void CalculateCostForFineTune(string integrationName, ModelFamilyType familyType, bool forTraining, int promptTokens, decimal price)
        {
            var openAiApi = _openAiFactory.Create(integrationName);
            try
            {
                var fineTuneCost = openAiApi.FineTune
                    .Create(integrationName)
                    .WithModel("test", familyType)
                    .CalculateCost(forTraining, promptTokens);
                Assert.Equal(price * promptTokens / 1000, fineTuneCost);
            }
            catch (Exception ex)
            {
                Assert.Equal("It's not yet implemented for Azure integration.", ex.Message);
            }
        }
        [Theory]
        [InlineData("", ImageSize.Large, 9, 0.02)]
        [InlineData("", ImageSize.Medium, 9, 0.018)]
        [InlineData("", ImageSize.Small, 10, 0.016)]
        public void CalculateCostForImage(string integrationName, ImageSize size, int numberOfResults, decimal price)
        {
            var openAiApi = _openAiFactory.Create(integrationName);
            var imageCost = openAiApi.Image
                .Generate("Something")
                .WithSize(size)
                .WithNumberOfResults(numberOfResults)
                .CalculateCost();
            Assert.Equal(price * numberOfResults, imageCost);
        }
        [Theory]
        [InlineData("", 9, 0.006)]
        [InlineData("", 5, 0.006)]
        [InlineData("", 10, 0.006)]
        public void CalculateCostForAudio(string integrationName, int minutes, decimal price)
        {
            var openAiApi = _openAiFactory.Create(integrationName);
            var memoryStream = new MemoryStream();
            var translationCost = openAiApi.Audio
                .Request(memoryStream, "name")
                .CalculateCostForTranslation(minutes);
            var transcriptionCost = openAiApi.Audio
                .Request(memoryStream, "name")
                .CalculateCostForTranscription(minutes);
            Assert.Equal(price * minutes, translationCost);
            Assert.Equal(price * minutes, transcriptionCost);
        }
        [Theory]
        [InlineData(ModelFamilyType.Gpt3_5, OpenAiType.Chat, "", 1, "This model is the perfect model for you.", 9, 0.002, 3, 5, "Gpt35Turbo", false)]
        [InlineData(ModelFamilyType.Gpt3_5, OpenAiType.Chat, "", 3, "Multiple models, each with different capabilities and price points. Prices are per 1000 tokens. You can think of tokens as pieces of words, where 1000 tokens is about 750 words.", 40, 0.002, 3, 5, "Gpt35Turbo", false)]
        [InlineData(ModelFamilyType.Gpt3_5, OpenAiType.Chat, "", 7, "Multiple models, each with different capabilities and price points. Prices are per 1000 tokens. You can think of tokens as pieces of words, where 1000 tokens is about 750 words.", 40, 0.002, 3, 5, "Gpt35Turbo", false)]
        [InlineData(ModelFamilyType.Ada, OpenAiType.Embedding, "", 1, "Fix the spelling mistakes", 4, 0.0004, 0, 0, "AdaTextEmbedding", true)]
        public async Task CalculateCosts(ModelFamilyType model, OpenAiType type, string integrationName, int times, string content, int numberOfTokens, decimal currentPrice, int startingAndEndingTokens, int fixedTokens, string modelType, bool imNotAbleToUnderstandHowToCalculateCompletionTokens)
        {
            var promptedTokens = times * numberOfTokens + (times * fixedTokens) + startingAndEndingTokens;
            var openAiApi = _openAiFactory.Create(integrationName);
            var manualCostCalculator = _openAiCost.Configure(configuration =>
            {
                configuration
                    .WithFamily(model)
                    .WithType(type);
            }, integrationName);
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
            Assert.True(finalPriceForEntireOperation >= price);
            if (!imNotAbleToUnderstandHowToCalculateCompletionTokens)
                Assert.Equal(finalPriceForEntireOperation, manualFinalCalculatedPrice);
        }
        private async Task<(Func<decimal> CostCalculatedBySystem, Func<decimal> CostCalculation, List<string> Contents, int PromptTokens, int CompletionTokens, IOpenAiTokenizer Tokenizer)> ExecuteRequestWithCostAsync(IOpenAi openAiApi, OpenAiType type, string content, int times, string modelType)
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
                            responseForChatWithCost.Result.Choices.Select(x => x.Message.Content as string).Where(x => x != null).ToList(),
                            responseForChatWithCost.Result.Usage.PromptTokens.Value,
                            responseForChatWithCost.Result.Usage.CompletionTokens.Value,
                            chatTokenizer);
                case OpenAiType.Embedding:
                    var embeddingModel = (EmbeddingModelType)Enum.Parse(typeof(EmbeddingModelType), modelType);
                    var embedding = openAiApi.Embedding
                     .Request(content)
                     .WithModel(embeddingModel);
                    var responseForEmbeddingWithCost = await embedding.ExecuteAndCalculateCostAsync();
                    var embeddingTokenizer = _utility.Tokenizer.WithEmbeddingModel(embeddingModel);
                    return (() => embedding.CalculateCost(),
                            () => responseForEmbeddingWithCost.CalculateCost(),
                            new List<string>(),
                            responseForEmbeddingWithCost.Result.Usage.PromptTokens.Value,
                            0,
                            embeddingTokenizer);
            }
            return default;
        }
    }
}
