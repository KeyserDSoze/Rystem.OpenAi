using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Test.Logger;
using Xunit;

namespace Rystem.OpenAi.Test.NonEndpointTests
{
    public class LoggerTests
    {
        private readonly IOpenAiFactory _openAiFactory;
        private readonly CustomLoggerMemory _customLoggerMemory;
        public LoggerTests(IOpenAiFactory openAiFactory,
            CustomLoggerMemory customLoggerMemory)
        {
            _openAiFactory = openAiFactory;
            _customLoggerMemory = customLoggerMemory;
        }
        [Fact]
        public async Task ExecuteAsync()
        {
            var openAiApi = _openAiFactory.Create("Azure");

            Assert.NotNull(openAiApi.Chat);
            _ = openAiApi.Chat
                .Request(new ChatMessage { Role = ChatRole.User, Content = "Hello!! How are you?" })
                .WithModel(ChatModelType.Gpt35Turbo)
                .WithTemperature(1)
                .CalculateCost();
            Assert.NotEmpty(_customLoggerMemory.Logs.Where(x => x.EventId.Id == 198_754));
        }
    }
}
