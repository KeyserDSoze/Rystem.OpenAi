using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class AudioEndpointTests
    {
        private readonly IOpenAiFactory _openAiFactory;
        public AudioEndpointTests(IOpenAiFactory openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        public async ValueTask CreateTranslationAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Audio);

            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;

            var results = await openAiApi.Audio
                .Request(editableFile, "default.mp3")
                .TranslateAsync();

            Assert.NotNull(results);
            Assert.True(results.Text.Length > 100);
        }

        [Theory]
        [InlineData("")]
        public async ValueTask CreateTranscriptionAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");

            var editableFile = new MemoryStream();
            readableStream.CopyTo(editableFile);
            editableFile.Position = 0;

            var results = await openAiApi.Audio
                .Request(editableFile, "default.mp3")
                .WithTemperature(1)
                .WithLanguage(Language.Italian)
                .WithPrompt("Incidente")
                .TranscriptAsync();

            Assert.NotNull(results);
            Assert.True(results.Text.Length > 100);
            Assert.StartsWith("Incidente tra due aerei di addestramento", results.Text);
        }
    }
}
