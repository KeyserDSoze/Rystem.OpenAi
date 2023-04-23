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
        [InlineData("")]
        [InlineData("Azure2")]
        public async ValueTask AllFlowAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Management);
            try
            {
                var createResponse = await openAiApi.Management.Deployment()
                    .WithCapacity(2)
                    .WithDeploymentTextModel("ada", TextModelType.AdaText)
                    .WithScaling(Management.DeploymentScaleType.Standard)
                    .CreateAsync();

                Assert.NotNull(createResponse);
                Assert.True(createResponse.CreatedAt > 1000);
            }
            catch (Exception ex)
            {
                var riseException = true;
                if (ex is MethodAccessException methodException && name == "")
                {
                    if (methodException.Message == "This method is valid only for Azure integration. Only Azure OpenAi has Deployment logic.")
                        riseException = false;
                }
                if (riseException)
                    throw ex;
            }
        }
    }
}
