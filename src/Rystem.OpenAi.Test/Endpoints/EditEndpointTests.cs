using System.Threading.Tasks;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class EditEndpointTests
    {
        private readonly IOpenAiFactory _openAiFactory;
        public EditEndpointTests(IOpenAiFactory openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        public async ValueTask CreateAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Edit);

            var results = await openAiApi.Edit
                .Request("Fix the spelling mistakes")
                .WithModel("testModel", ModelFamilyType.Davinci)
                .WithModel(EditModelType.TextDavinciEdit)
                .SetInput("What day of the wek is it?")
                .WithTemperature(0.5)
                .ExecuteAsync();

            Assert.NotNull(results);
            Assert.NotNull(results.CreatedUnixTime);
            Assert.True(results.CreatedUnixTime.Value != 0);
            Assert.NotNull(results.Created);
            Assert.NotNull(results.Choices);
            Assert.True(results.Choices.Count != 0);
            Assert.Contains(results.Choices, c => c.Text.StartsWith("What day of the week is it?"));
        }
    }
}
