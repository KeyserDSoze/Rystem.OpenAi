using System.Threading.Tasks;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class SetupTests
    {
        [Theory]
        [InlineData(OpenAiType.AudioTranscription, null, false, null, "https://api.openai.com/v2/audio/transcriptions")]
        public void SetupOpenAi(OpenAiType type, string modelId, bool forced, string appendBeforeQuery, string uri)
        {
            var settings = new OpenAiSettings()
            {
                ApiKey = "apiKeyForTest",
            };
            settings
                .UseVersionForAudioTranscription("v2")
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
    }
}
