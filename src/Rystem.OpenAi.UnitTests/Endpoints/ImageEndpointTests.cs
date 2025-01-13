using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Image;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ImageEndpointTests
    {
        private readonly IFactory<IOpenAi> _openAiFactory;
        public ImageEndpointTests(IFactory<IOpenAi> openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure4")]
        public async ValueTask CreateAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Image);

            var response = await openAiApi.Image
                .WithSize(ImageSize.Large)
                .GenerateAsync("Create a captive logo with ice and fire, and thunder with the word Rystem. With a desolated futuristic landscape.");

            var uri = response.Data?.FirstOrDefault();
            Assert.NotNull(uri);
            Assert.Contains("blob.core.windows.net", uri.Url);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure4")]
        public async ValueTask CreateWithBase64Async(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Image);

            var response = await openAiApi.Image
                .WithSize(ImageSize.Large)
                .GenerateAsBase64Async("Create a captive logo with ice and fire, and thunder with the word Rystem. With a desolated futuristic landscape.");

            var image = response.Data?.FirstOrDefault();
            Assert.NotNull(image);
            var imageAsStream = image.ConvertToStream();
            Assert.NotNull(imageAsStream);
        }
        [Theory]
        [InlineData("")]
        [InlineData("AzureForDalle2")]
        public async ValueTask EditAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Image);
            //var location = Assembly.GetExecutingAssembly().Location;
            //location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            //using var readableStream = File.OpenRead($"{location}\\Files\\cuteotter.png");
            var editableFile = new MemoryStream();
            //await readableStream.CopyToAsync(editableFile);
            var creation = await openAiApi.Image
                .WithModel(ImageModelName.Dalle2)
                .WithSize(ImageSize.Small)
                .GenerateAsBase64Async("A cute baby sea otter.");
            await creation!.Data!.First().ConvertToStream()!.CopyToAsync(editableFile);
            editableFile.Position = 0;
            var response = await openAiApi.Image
                //.WithMask()//new ImageRange(new System.Range(1, 3), new System.Range(4, 6))
                .WithModel(ImageModelName.Dalle2)
                .WithSize(ImageSize.Small)
                .WithNumberOfResults(2)
                .EditAsync("A cute baby sea otter wearing a beret", editableFile, "cuteotter.png");

            var uri = response.Data?.FirstOrDefault();
            Assert.Equal(2, response.Data?.Count);
            Assert.NotNull(uri);
            Assert.Contains("blob.core.windows.net", uri.Url);
        }
        [Theory]
        [InlineData("")]
        [InlineData("AzureForDalle2")]
        public async ValueTask VariateAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Image);

            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\otter.png");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;
            var response = await openAiApi.Image
                .WithModel(ImageModelName.Dalle2)
                .WithSize(ImageSize.Small)
                .WithNumberOfResults(1)
                .VariateAsync(editableFile, "otter.png");

            var uri = response.Data?.FirstOrDefault();
            Assert.NotEmpty(response.Data ?? []);
            Assert.NotNull(uri);
            Assert.Contains("blob.core.windows.net", uri.Url);
        }
    }
}
