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
        private readonly IOpenAiFactory _openAiFactory;
        public ImageEndpointTests(IOpenAiFactory openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        public async ValueTask CreateAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Image);

            var response = await openAiApi.Image
                .Generate("Create a captive logo with ice and fire, and thunder with the word Rystem. With a desolated futuristic landscape.")
                .WithSize(ImageSize.Large)
                .ExecuteAsync();

            var uri = response.Data.FirstOrDefault();
            Assert.NotNull(uri);
            Assert.Contains("blob.core.windows.net", uri.Url);
        }
        [Theory]
        [InlineData("")]
        public async ValueTask CreateWithBase64Async(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Image);

            var response = await openAiApi.Image
                .Generate("Create a captive logo with ice and fire, and thunder with the word Rystem. With a desolated futuristic landscape.")
                .WithSize(ImageSize.Large)
                .ExecuteWithBase64Async();

            var image = response.Data.FirstOrDefault();
            Assert.NotNull(image);
            var imageAsImage = image.ConvertToImage();
            Assert.NotNull(imageAsImage);
            var imageAsStream = image.ConvertToStream();
            Assert.NotNull(imageAsStream);
        }
        [Theory]
        [InlineData("")]
        public async ValueTask CreateAndDownloadAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Image);

            var streams = new List<Stream>();
            await foreach (var image in openAiApi.Image
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
        [Theory]
        [InlineData("")]
        public async ValueTask EditAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Image);

            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\otter.png");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;

            var response = await openAiApi.Image
                .Generate("A cute baby sea otter wearing a beret")
                .EditAndTrasformInPng(editableFile, "otter.png")
                .WithMask(new ImageRange(new System.Range(1, 3), new System.Range(4, 6)))
                .WithSize(ImageSize.Small)
                .WithNumberOfResults(2)
                .ExecuteAsync();

            var uri = response.Data.FirstOrDefault();
            Assert.Equal(2, response.Data.Count);
            Assert.NotNull(uri);
            Assert.Contains("blob.core.windows.net", uri.Url);
        }
        [Theory]
        [InlineData("")]
        public async ValueTask VariateAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Image);

            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\otter.png");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;
            var response = await openAiApi.Image
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
