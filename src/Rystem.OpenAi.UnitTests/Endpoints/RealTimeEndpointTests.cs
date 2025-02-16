using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class RealTimeEndpointTests
    {
        private readonly IFactory<IOpenAi> _openAiFactory;

        public RealTimeEndpointTests(IFactory<IOpenAi> openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure6")]
        public async ValueTask AllAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.RealTime);
            var session = await openAiApi.RealTime.CreateSessionAsync();
            Assert.NotNull(session);
            Assert.NotNull(session.ClientSecret?.Value);
            var client = openAiApi.RealTime.GetClient(session.ClientSecret.Value);
            Assert.NotNull(client);
            await client.ConnectAsync();
            await client.DisconnectAsync();
        }
    }
}
