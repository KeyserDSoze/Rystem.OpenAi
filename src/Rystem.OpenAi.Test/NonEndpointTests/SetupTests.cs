using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class SetupTests
    {
        [Theory]
        [InlineData(OpenAiType.AudioTranscription, null, false, null, "https://api.openai.com/v3/audio/transcriptions")]
        public void SetupOpenAi(OpenAiType type, string modelId, bool forced, string appendBeforeQuery, string uri)
        {
            var settings = new OpenAiSettings()
            {
                ApiKey = "apiKeyForTest",
            };
            settings
                .UseVersionForAudioTranscription("v2")
                .UseVersionForAudioTranscription("v3")
                .UseVersionForChat("v2")
                .UseVersionForCompletion("v2")
                .UseVersionForEdit("v2")
                .UseVersionForEmbedding("v2")
                .UseVersionForFile("v2")
                .UseVersionForFineTune("v2")
                .UseVersionForImage("v2")
                .UseVersionForModel("v2")
                .UseVersionForModeration("v2")
                .UseVersionForAudioTranslation("v2");
            var oac = new OpenAiConfiguration(settings, string.Empty);
            oac.ConfigureEndpoints();
            var apiUri = oac.GetUri(type, modelId, forced, appendBeforeQuery);
            Assert.Equal(uri, apiUri);
        }
        [Fact]
        public void AddApiKeyNull()
        {
            var serviceCollection = new ServiceCollection();
            try
            {
                serviceCollection.AddOpenAi(x =>
                {
                    x.ApiKey = null;
                });
            }
            catch (Exception ex)
            {
                Assert.Equal("Value cannot be null. (Parameter 'ApiKey is empty.')", ex.Message);
            }
        }
    }
}
