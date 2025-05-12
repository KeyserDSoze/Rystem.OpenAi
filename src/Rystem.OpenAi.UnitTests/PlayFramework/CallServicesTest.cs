using Xunit;

namespace Rystem.PlayFramework.Test
{
    public sealed class CallServicesTest
    {
        private readonly ISceneManager _sceneManager;

        public CallServicesTest(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }
        [Theory]
        [InlineData("Che tempo fa oggi a Milano?", "")]
        [InlineData("Qual è il nome dell'utente con username keysersoze?", "Alessandro Rapiti")]
        public async ValueTask TestAsync(string message, string expectedSubstring)
        {
            var response = _sceneManager.ExecuteAsync(message, null, TestContext.Current.CancellationToken);
            string? id = null;
            var responses = new List<AiSceneResponse>();
            await foreach (var item in response)
            {
                id ??= item.RequestKey;
                Assert.Equal(id, item.RequestKey);
                Assert.Contains(expectedSubstring, item.RequestKey);
                responses.Add(item);
            }
            response = _sceneManager.ExecuteAsync(message, settings => settings.WithKey(id), TestContext.Current.CancellationToken);
            var responses2 = new List<AiSceneResponse>();
            await foreach (var item in response)
            {
                Assert.Equal(id, item.RequestKey);
                Assert.Contains(expectedSubstring, item.RequestKey);
                responses2.Add(item);
            }
            Assert.True(responses.Count > responses2.Count, "The second response should be smaller than the first one.");
        }
        [Theory]
        [InlineData("Vorrei prendere dal 10 di giugno al 20", "")]
        public async ValueTask TestVacationAsync(string message, string expectedSubstring)
        {
            var response = _sceneManager.ExecuteAsync(message, null, TestContext.Current.CancellationToken);
            string? id = null;
            var responses = new List<AiSceneResponse>();
            await foreach (var item in response)
            {
                id ??= item.RequestKey;
                Assert.Equal(id, item.RequestKey);
                Assert.Contains(expectedSubstring, item.RequestKey);
                responses.Add(item);
            }
            response = _sceneManager.ExecuteAsync(message, settings => settings.WithKey(id), TestContext.Current.CancellationToken);
            var responses2 = new List<AiSceneResponse>();
            await foreach (var item in response)
            {
                Assert.Equal(id, item.RequestKey);
                Assert.Contains(expectedSubstring, item.RequestKey);
                responses2.Add(item);
            }
            Assert.True(responses.Count > responses2.Count, "The second response should be smaller than the first one.");
        }
    }
}
