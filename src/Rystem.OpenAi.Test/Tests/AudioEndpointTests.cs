using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class AudioEndpointTests
    {
        private readonly IOpenAiApi _openAiApi;
        public AudioEndpointTests(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        [Fact]
        public async ValueTask CreateTranslationAsync()
        {
            Assert.NotNull(_openAiApi.Audio);

            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;

            var results = await _openAiApi.Audio
                .Request(editableFile, "default.mp3")
                .TranslateAsync();

            Assert.NotNull(results);
            Assert.True(results.Text.Length > 100);
        }

        [Fact]
        public async ValueTask CreateTranscriptionAsync()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");

            var editableFile = new MemoryStream();
            readableStream.CopyTo(editableFile);
            editableFile.Position = 0;

            var results = await _openAiApi.Audio
                .Request(editableFile, "default.mp3")
                .TranscriptAsync();

            Assert.NotNull(results);
            Assert.True(results.Text.Length > 100);
            Assert.StartsWith("Incidente tra due aerei di addestramento", results.Text);
        }
    }
}
