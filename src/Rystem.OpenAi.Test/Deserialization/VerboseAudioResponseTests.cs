using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Rystem.OpenAi.Image;
using Xunit;

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
