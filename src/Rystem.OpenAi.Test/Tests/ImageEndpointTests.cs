using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Rystem.OpenAi.Image;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ImageEndpointTests
    {
        private readonly IOpenAiApi _openAiApi;
        public ImageEndpointTests(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        [Fact]
        public async ValueTask CreateAsync()
        {
            Assert.NotNull(_openAiApi.Image);

            var response = await _openAiApi.Image
                .Generate("Create a captive logo with ice and fire for my brand Rystem")
                .WithSize(ImageSize.Large)
                .ExecuteAsync();

            var uri = response.Data.FirstOrDefault();
            Assert.NotNull(uri);
            Assert.Contains("blob.core.windows.net", uri.Url);
        }
        [Fact]
        public async ValueTask CreateAndDownloadAsync()
        {
            Assert.NotNull(_openAiApi.Image);

            var streams = new List<Stream>();
            await foreach (var image in _openAiApi.Image
                .Generate("A cute baby sea otter")
                .WithSize(ImageSize.Small)
                .DownloadAsync())
            {
                streams.Add(image);
            }

            Assert.NotEmpty(streams);
            var firstImage = streams.First();
            Assert.True(firstImage.Length > 0);
            var memoryStream = new MemoryStream();
            await firstImage.CopyToAsync(memoryStream);
            Assert.True(memoryStream.Length == firstImage.Length);
        }
        [Fact]
        public async ValueTask EditAsync()
        {
            Assert.NotNull(_openAiApi.Image);

            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\otter.png");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;

            var response = await _openAiApi.Image
                .Generate("A cute baby sea otter wearing a beret")
                .EditAndTrasformInPng(editableFile, "otter.png")
                .WithSize(ImageSize.Small)
                .WithNumberOfResults(2)
                .ExecuteAsync();

            var uri = response.Data.FirstOrDefault();
            Assert.Equal(2, response.Data.Count);
            Assert.NotNull(uri);
            Assert.Contains("blob.core.windows.net", uri.Url);
        }
        [Fact]
        public async ValueTask VariateAsync()
        {
            Assert.NotNull(_openAiApi.Image);

            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\otter.png");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;
            var response = await _openAiApi.Image
                .VariateAndTransformInPng(editableFile, "otter.png")
                .WithSize(ImageSize.Small)
                .WithNumberOfResults(1)
                .ExecuteAsync();

            var uri = response.Data.FirstOrDefault();
            Assert.NotEmpty(response.Data);
            Assert.NotNull(uri);
            Assert.Contains("blob.core.windows.net", uri.Url);
        }
    }
}
