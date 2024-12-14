using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Audio;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class AudioEndpointTests
    {
        private readonly IFactory<IOpenAi> _openAiFactory;
        public AudioEndpointTests(IFactory<IOpenAi> openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Fact]
        public void SetupError()
        {
            var openAiApi = _openAiFactory.Create("")!;
            var request = openAiApi.Audio;
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
        [InlineData("Azure3")]
        public async ValueTask CreateTranslationAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Audio);

            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;

            var results = await openAiApi.Audio
                .WithTemperature(1)
                .WithPrompt("sample")
                .WithFile(editableFile.ToArray(), "default.mp3")
                .TranslateAsync();

            Assert.NotNull(results);
            Assert.True(results.Text?.Length > 100);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Azure3")]
        public async ValueTask CreateVerboseTranslationAsSegmentsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Audio);

            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;

            var results = await openAiApi.Audio
                .WithFile(editableFile.ToArray(), "default.mp3")
                .WithTemperature(1)
                .WithPrompt("sample")
                .VerboseTranslateAsSegmentsAsync();

            Assert.NotNull(results);
            Assert.True(results.Text?.Length > 100);
            Assert.NotEmpty(results.Segments ?? []);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Azure3")]
        public async ValueTask CreateVerboseTranslationAsWordsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Audio);

            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;

            var results = await openAiApi.Audio
                .WithFile(editableFile.ToArray(), "default.mp3")
                .WithTemperature(1)
                .WithPrompt("sample")
                .VerboseTranslateAsWordsAsync();

            Assert.NotNull(results);
            Assert.True(results.Text?.Length > 100);
            Assert.NotEmpty(results.Words ?? []);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure3")]
        public async ValueTask CreateTranscriptionAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");

            var editableFile = new MemoryStream();
            readableStream.CopyTo(editableFile);
            editableFile.Position = 0;

            var results = await openAiApi.Audio
                .WithFile(editableFile.ToArray(), "default.mp3")
                .WithTemperature(1)
                .WithLanguage(Language.Italian)
                .WithPrompt("Incidente")
                .TranscriptAsync();

            Assert.NotNull(results);
            Assert.True(results.Text?.Length > 100);
            Assert.StartsWith("Incidente tra due aerei di addestramento", results.Text);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Azure3")]
        public async ValueTask CreateVerboseTranscriptionAsSegmentsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");

            var editableFile = new MemoryStream();
            readableStream.CopyTo(editableFile);
            editableFile.Position = 0;

            var results = await openAiApi.Audio
                .WithFile(editableFile.ToArray(), "default.mp3")
                .WithTemperature(1)
                .WithLanguage(Language.Italian)
                .WithPrompt("Incidente")
                .VerboseTranscriptAsSegmentsAsync();

            Assert.NotNull(results);
            Assert.True(results.Text?.Length > 100);
            Assert.StartsWith("Incidente tra due aerei di addestramento", results.Text);
            Assert.NotEmpty(results.Segments ?? []);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure3")]
        public async ValueTask CreateVerboseTranscriptionAsWordsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");

            var editableFile = new MemoryStream();
            readableStream.CopyTo(editableFile);
            editableFile.Position = 0;

            var results = await openAiApi.Audio
                .WithFile(editableFile.ToArray(), "default.mp3")
                .WithTemperature(1)
                .WithLanguage(Language.Italian)
                .WithPrompt("Incidente")
                .VerboseTranscriptAsWordsAsync();

            Assert.NotNull(results);
            Assert.True(results.Text?.Length > 100);
            Assert.StartsWith("Incidente tra due aerei di addestramento", results.Text);
            Assert.NotEmpty(results.Words ?? []);
        }
        [Theory]
        [InlineData("", "Hello world!")]
        [InlineData("Azure3", "Hello world!")]
        public async ValueTask CreateSpeechAsync(string name, string text)
        {
            var openAiApi = _openAiFactory.Create(name)!;

            var result = await openAiApi.Speech
                .WithVoice(AudioVoice.Fable)
                .WithSpeed(1.3d)
                .Mp3Async(text);

            Assert.NotNull(result);
            Assert.True(result.Length > 100);
        }
    }
}
