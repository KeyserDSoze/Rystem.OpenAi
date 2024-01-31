using System.Text.Json;
using Rystem.OpenAi.Image;

namespace Rystem.OpenAi.UnitTests
{
    public class VerboseAudioResponseTests
    {
        const string ExampleResponse = "Files/verbose-audio-response.json";

        [Fact]
        public async Task CanDeserializeFromExample()
        {
            var json = await File.ReadAllTextAsync(ExampleResponse);

            var verbose = JsonSerializer.Deserialize<VerboseAudioResult>(json);

            Assert.NotNull(verbose);
            Assert.InRange(
                verbose.Duration,
                TimeSpan.FromSeconds(31.7),
                TimeSpan.FromSeconds(31.8)
            );
            Assert.Equal(7, verbose.Segments?.Length);
        }
    }
}