using System;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ManagementTests
    {
        private readonly IOpenAiFactory _openAiFactory;
        public ManagementTests(IOpenAiFactory openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        public async Task GetAsync(string integrationName)
        {
            var management = _openAiFactory.CreateManagement(integrationName);
            var usages = await management
                .Billing
                .From(new DateTime(2023, 4, 1))
                .To(new DateTime(2023, 4, 30))
                .GetUsageAsync();
            Assert.NotEmpty(usages.DailyCosts);
        }
    }
}
