using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class DeploymentsTest
    {
        private readonly IOpenAiFactory _openAiFactory;
        public DeploymentsTest(IOpenAiFactory openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("Azure2")]
        public async ValueTask AllFlowAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Management);

            var createResponse = await openAiApi.Management.Deployment()
                .WithCapacity(2)
                .WithDeploymentTextModel("ada", TextModelType.AdaText)
                .WithScaling(Management.DeploymentScaleType.Standard)
                .CreateAsync();

            Assert.NotNull(createResponse);
            Assert.True(createResponse.CreatedAt > 1000);
        }
    }
}
