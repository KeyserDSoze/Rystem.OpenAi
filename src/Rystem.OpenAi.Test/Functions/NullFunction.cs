using System;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class NullFunction : IOpenAiChatFunction
    {
        public const string NameLabel = "get_current_cart";
        public string Name => NameLabel;
        private const string DescriptionLabel = "Get the current cart of your user";
        public string Description => DescriptionLabel;
        public Type Input => typeof(NullRequestModel);
        public Task<object> WrapAsync(string message)
        {
            _ = System.Text.Json.JsonSerializer.Deserialize<NullRequestModel>(message);
            return Task.FromResult(default(object));
        }
    }
}
