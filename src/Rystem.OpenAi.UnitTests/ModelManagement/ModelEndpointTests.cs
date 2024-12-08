﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ModelEndpointTests
    {
        private readonly IFactory<IOpenAi> _openAiFactory;
        private readonly IOpenAiUtility _openAiUtility;

        public ModelEndpointTests(IFactory<IOpenAi> openAiFactory, IOpenAiUtility openAiUtility)
        {
            _openAiFactory = openAiFactory;
            _openAiUtility = openAiUtility;
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask GetAllModelsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Model);
            var results = await openAiApi.Model.ListAsync();
            foreach (var model in results.Models ?? [])
            {
                var modelResult = await openAiApi.Model.RetrieveAsync(model.Id!);
                Assert.NotNull(modelResult);
                Assert.Equal(model.Id, modelResult.AsString());
                //if (string.IsNullOrWhiteSpace(name))
                //    Assert.True(modelResult.Created > new DateTime(2018, 1, 1));
            }
            Assert.NotNull(results);
            Assert.NotEmpty(results.Models ?? []);
            Assert.Contains(results.Models ?? [], c => c.Id?.ToLower().StartsWith("text-davinci") == true);
        }

        [Theory]
        [InlineData("")]
        public async ValueTask GetModelDetailsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Model);

            var result = await openAiApi.Model.RetrieveAsync(ChatModelName.ChatGpt_4o_latest);
            Assert.NotNull(result);

            //Assert.NotNull(result.CreatedUnixTime);
            //Assert.True(result.CreatedUnixTime.Value != 0);
            //Assert.NotNull(result.Created);
            //Assert.True(result.Created.Value > new DateTime(2018, 1, 1));
            //Assert.True(result.Created.Value < DateTime.Now.AddDays(1));

            Assert.NotNull(result.AsString());
            //Assert.NotNull(result.OwnedBy);
            //Assert.Equal(TextModelType.DavinciText3.ToModel(), result.Id.ToLower());
        }


        [Theory]
        [InlineData("")]
        public async ValueTask GetEnginesAsync_ShouldReturnTheEngineList(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            var models = await openAiApi.Model.ListAsync();
            Assert.True(models.Models?.Count > 5);
        }

        [Theory]
        [InlineData("ada", "")]
        [InlineData("babbage", "")]
        [InlineData("curie", "")]
        [InlineData("davinci", "")]
        public async ValueTask RetrieveEngineDetailsAsync_ShouldRetrieveEngineDetails(string modelId, string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            var modelData = await openAiApi.Model.RetrieveAsync(modelId);
            Assert.Equal(modelId, modelData.AsString());
            //Assert.True(modelData.Created > new DateTime(2018, 1, 1));
            //Assert.True(modelData.Created < DateTime.UtcNow.AddDays(1));
        }
    }
}
