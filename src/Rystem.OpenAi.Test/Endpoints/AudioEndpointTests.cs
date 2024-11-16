using System;
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
        [Fact]
        public void SetupError()
        {
            var openAiApi = _openAiFactory.Create("");
            var request = openAiApi.Audio
                .Request(new MemoryStream(), "default.mp3");
            try
            {
                request
                    .WithTemperature(-5);
            }
            catch (Exception ex)
            {
                Assert.Equal("Temperature with a value lesser than 0", ex.Message);
            }
            try
            {
                request
                    .WithTemperature(1.1);
            }
            catch (Exception ex)
            {
                Assert.Equal("Temperature with a value greater than 1", ex.Message);
            }
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
                .WithTemperature(1)
                .WithPrompt("sample")
                .TranslateAsync();

            Assert.NotNull(results);
            Assert.True(results.Text.Length > 100);
        }

        [Theory]
        [InlineData("")]
        public async ValueTask CreateVerboseTranslationAsync(string name)
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
                .WithTemperature(1)
                .WithPrompt("sample")
                .VerboseTranslateAsync();

            Assert.NotNull(results);
            Assert.True(results.Text.Length > 100);
            Assert.NotEmpty(results.Segments);
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

        [Theory]
        [InlineData("")]
        public async ValueTask CreateVerboseTranscriptionAsync(string name)
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
                .VerboseTranscriptAsync();

            Assert.NotNull(results);
            Assert.True(results.Text.Length > 100);
            Assert.StartsWith("Incidente tra due aerei di addestramento", results.Text);
            Assert.NotEmpty(results.Segments);
        }
        [Theory]
        [InlineData("", "Hello world!")]
        public async ValueTask CreateSpeechAsync(string name, string text)
        {
            var openAiApi = _openAiFactory.Create(name);

            var result = await openAiApi.Audio
                .Speech(text)
                .WithVoice(AudioVoice.Fable)
                .WithSpeed(1.3d)
                .Mp3Async();

            Assert.NotNull(result);
            Assert.True(result.Length > 100);
        }
    }
}
