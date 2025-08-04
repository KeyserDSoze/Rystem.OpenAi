using Xunit;

namespace Rystem.PlayFramework.Test
{
    public sealed class CallServicesTest
    {
        private readonly ISceneManager _sceneManager;
        private readonly ISceneManager _sceneManager2;
        private readonly ISceneManager _sceneManager3;

        public CallServicesTest(ISceneManager sceneManager, ISceneManager sceneManager2, ISceneManager sceneManager3)
        {
            _sceneManager = sceneManager;
            _sceneManager2 = sceneManager2;
            _sceneManager3 = sceneManager3;
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
                responses.Add(item);
            }
            Assert.Contains(responses, item => item.Message?.Contains(expectedSubstring) == true);
            response = _sceneManager2.ExecuteAsync(message, settings => settings.WithKey(id), TestContext.Current.CancellationToken);
            var responses2 = new List<AiSceneResponse>();
            await foreach (var item in response)
            {
                Assert.Equal(id, item.RequestKey);
                responses2.Add(item);
            }
            Assert.Contains(responses2, item => item.Message?.Contains(expectedSubstring) == true);
            Assert.True(responses.Count > responses2.Count, "The second response should be smaller than the first one.");
            response = _sceneManager3.ExecuteAsync(message, settings => settings.WithKey(id), TestContext.Current.CancellationToken);
            var responses3 = new List<AiSceneResponse>();
            await foreach (var item in response)
            {
                Assert.Equal(id, item.RequestKey);
                responses3.Add(item);
            }
            Assert.Contains(responses3, item => item.Message?.Contains(expectedSubstring) == true);
            Assert.True(responses.Count > responses3.Count, "The third response should be smaller than the first one.");
        }
        [Theory]
        [InlineData("Vorrei prendere dal 10 di giugno al 20", "Si, puoi inviare la richiesta")]
        public async ValueTask TestVacationAsync(string message, string secondMessage)
        {
            var response = _sceneManager.ExecuteAsync(message, null, TestContext.Current.CancellationToken);
            string? id = null;
            var responses = new List<AiSceneResponse>();
            await foreach (var item in response)
            {
                id ??= item.RequestKey;
                Assert.Equal(id, item.RequestKey);
                responses.Add(item);
            }
            response = _sceneManager.ExecuteAsync(secondMessage, settings => settings.WithKey(id), TestContext.Current.CancellationToken);
            var responses2 = new List<AiSceneResponse>();
            await foreach (var item in response)
            {
                Assert.Equal(id, item.RequestKey);
                responses2.Add(item);
            }
            Assert.True(responses.Count > responses2.Count, "The second response should be smaller than the first one.");
        }
    }
}
