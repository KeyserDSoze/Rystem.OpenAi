﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ModelEndpointTests
    {
        private readonly IFactory<IOpenAi> _openAiFactory;

        public ModelEndpointTests(IFactory<IOpenAi> openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        public async ValueTask GetAllModelsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Model);
            var results = await openAiApi.Model.ListAsync();
            foreach (var model in results.Data ?? [])
            {
                var modelResult = await openAiApi.Model.RetrieveAsync(model.Id!);
                Assert.NotNull(modelResult);
                Assert.Equal(model.Id, modelResult.Id);
                //if (string.IsNullOrWhiteSpace(name))
                //    Assert.True(modelResult.Created > new DateTime(2018, 1, 1));
            }
            Assert.NotNull(results);
            Assert.NotEmpty(results.Data ?? []);
        }

        [Theory]
        [InlineData("")]
        public async ValueTask GetModelDetailsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Model);

            var result = await openAiApi.Model.RetrieveAsync(ChatModelName.Gpt4_o);
            Assert.NotNull(result);

            //Assert.NotNull(result.CreatedUnixTime);
            //Assert.True(result.CreatedUnixTime.Value != 0);
            //Assert.NotNull(result.Created);
            //Assert.True(result.Created.Value > new DateTime(2018, 1, 1));
            //Assert.True(result.Created.Value < DateTime.Now.AddDays(1));

            Assert.NotNull(result.Id);
            //Assert.NotNull(result.OwnedBy);
            //Assert.Equal(TextModelType.DavinciText3.ToModel(), result.Id.ToLower());
        }


        [Theory]
        [InlineData("")]
        public async ValueTask GetEnginesAsync_ShouldReturnTheEngineList(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            var models = await openAiApi.Model.ListAsync();
            Assert.True(models.Data?.Count > 5);
        }
    }
}
