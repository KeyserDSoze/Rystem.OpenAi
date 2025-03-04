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
            var isOnAzure = name.Contains("Azure");
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.RealTime);
            var session = isOnAzure ? null : await openAiApi.RealTime.CreateSessionAsync();
            var client = isOnAzure ?
                await openAiApi.RealTime.GetAuthenticatedClientAsync() :
                openAiApi.RealTime.GetClientWithEphemeralKey(session!.ClientSecret!.Value!);
            Assert.NotNull(client);
            await client.ConnectAsync();
            var itemCreator = client.ConversationItemCreate(null);
            itemCreator.WithUserMessage("Hello");
            await itemCreator.SendAsync();
            await client.DisconnectAsync();
        }
    }
}
