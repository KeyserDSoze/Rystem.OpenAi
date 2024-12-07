using Xunit;

namespace Rystem.PlayFramework.Test
{
    public sealed class ApiCallTest
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ApiCallTest(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [Theory]
        [InlineData("Che tempo fa oggi a Milano?", "")]
        [InlineData("Qual è il nome dell'utente con username keysersoze?", "Alessandro Rapiti")]
        public async ValueTask TestAsync(string message, string expectedSubstring)
        {
            var client = _httpClientFactory.CreateClient("client");
            var response = await client.GetAsync($"api/ai/message?m={message}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains(expectedSubstring, content);
        }
    }
}
