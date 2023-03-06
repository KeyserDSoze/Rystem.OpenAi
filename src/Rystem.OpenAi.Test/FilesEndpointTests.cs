using System;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi;
using Xunit;

namespace Azure.OpenAi.Test
{
    public class FilesEndpointTests
    {
        private readonly IOpenAiApi _openAiApi;
        public FilesEndpointTests(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        //[Fact]
        //[Order(1)]
        //public async Task UploadFile()
        //{
        //    var api = DiUtility.GetOpenAi();
        //    var response = await api.Files.UploadFileAsync("data-test-file.jsonl");
        //    Assert.NotNull(response);
        //    Assert.IsTrue(response.Id.Length > 0);
        //    Assert.IsTrue(response.Object == "file");
        //    Assert.IsTrue(response.Bytes > 0);
        //    Assert.IsTrue(response.CreatedAt > 0);
        //    Assert.IsTrue(response.Status == "uploaded");
        //    // The file must be processed before it can be used in other operations, so for testing purposes we just sleep awhile.
        //    Thread.Sleep(10000);
        //}

        //[Fact]
        //[Order(2)]
        //public async Task ListFiles()
        //{
        //    var api = DiUtility.GetOpenAi();
        //    var response = await api.Files.GetFilesAsync();

        //    foreach (var file in response)
        //    {
        //        Assert.NotNull(file);
        //        Assert.IsTrue(file.Id.Length > 0);
        //    }
        //}


        //[Fact]
        //[Order(3)]
        //public async Task GetFile()
        //{
        //    var api = DiUtility.GetOpenAi();
        //    var response = await api.Files.GetFilesAsync();
        //    foreach (var file in response)
        //    {
        //        Assert.NotNull(file);
        //        Assert.IsTrue(file.Id.Length > 0);
        //        string id = file.Id;
        //        if (file.Name == "fine-tuning-data.jsonl")
        //        {
        //            var fileResponse = await api.Files.GetFileAsync(file.Id);
        //            Assert.NotNull(fileResponse);
        //            Assert.IsTrue(fileResponse.Id == id);
        //        }
        //    }
        //}

        //[Fact]
        //[Order(4)]
        //public async Task DeleteFiles()
        //{
        //    var api = DiUtility.GetOpenAi();
        //    var response = await api.Files.GetFilesAsync();
        //    foreach (var file in response)
        //    {
        //        Assert.NotNull(file);
        //        Assert.IsTrue(file.Id.Length > 0);
        //        if (file.Name == "fine-tuning-data.jsonl")
        //        {
        //            var deleteResponse = await api.Files.DeleteFileAsync(file.Id);
        //            Assert.NotNull(deleteResponse);
        //            Assert.IsTrue(deleteResponse.Deleted);
        //        }
        //    }
        //}

    }
}
