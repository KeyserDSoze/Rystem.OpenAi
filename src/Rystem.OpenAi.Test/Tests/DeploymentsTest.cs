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
        [InlineData("", null)]
        [InlineData("Azure2", null)]
        [InlineData("Azure2", "test")]
        public async ValueTask AllFlowAsync(string name, string deploymentId)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Management);
            if (name == "")
            {
                try
                {
                    _ = openAiApi.Management.Deployment;
                }
                catch (Exception ex)
                {
                    Assert.Equal("This method is valid only for Azure integration. Only Azure OpenAi has Deployment logic.", ex.Message);
                }
            }
            else
            {
                foreach (var deployment in (await openAiApi.Management.Deployment.ListAsync()).Data)
                {
                    await openAiApi.Management.Deployment.DeleteAsync(deployment.Id);
                }

                var createResponse = await openAiApi.Management.Deployment
                    .Create(deploymentId)
                    .WithCapacity(2)
                    .WithDeploymentTextModel("ada", TextModelType.AdaText)
                    .WithScaling(Management.DeploymentScaleType.Standard)
                    .ExecuteAsync();

                Assert.NotNull(createResponse);
                Assert.True(createResponse.CreatedAt > 1000);

                var deploymentResult = await openAiApi.Management.Deployment.RetrieveAsync(createResponse.Id);
                Assert.NotNull(deploymentResult);

                var listResponse = await openAiApi.Management.Deployment.ListAsync();
                Assert.NotEmpty(listResponse.Succeeded);

                try
                {
                    var updateResponse = await openAiApi.Management.Deployment
                        .Update(createResponse.Id)
                        .WithCapacity(1)
                        .WithDeploymentTextModel("ada", TextModelType.AdaText)
                        .WithScaling(Management.DeploymentScaleType.Standard)
                        .ExecuteAsync();
                    Assert.NotNull(updateResponse);
                }
                catch (Exception ex)
                {
                    Assert.Empty(ex.Message);
                }

                var deleteResponse = await openAiApi.Management.Deployment
                    .DeleteAsync(createResponse.Id);
                Assert.True(deleteResponse);

                listResponse = await openAiApi.Management.Deployment.ListAsync();
                Assert.Empty(listResponse.Succeeded);
            }
        }
        [Theory]
        [InlineData("Azure")]
        public async ValueTask ListAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);

            foreach (var deployment in (await openAiApi.Management.Deployment.ListAsync()).Data)
            {
                Assert.NotNull(deployment);
            }
        }
    }
}
