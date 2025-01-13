using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Image
{
    internal sealed class OpenAiImage : OpenAiBuilder<IOpenAiImage, ImageRequest, ImageModelName>, IOpenAiImage
    {
        private Stream? _mask;
        private string? _maskName;
        private ImageSize _size;
        private ImageQuality _quality;
        public OpenAiImage(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLogger logger)
            : base(factory, configurationFactory, logger, OpenAiType.Image)
        {
            Request.Model = ImageModelName.Dalle3;
            Request.NumberOfResults = 1;
            Request.ResponseFormat = FormatResultImage.Url.AsString();
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.Image != null)
            {
                configuration.Settings.DefaultRequestConfiguration.Image.Invoke(this);
            }
        }
        private void CheckPromptLength(string prompt)
        {
            if (Request.Model == ImageModelName.Dalle2 && prompt.Length > 1000)
                throw new ArgumentOutOfRangeException(nameof(prompt), "The maximum character length for the prompt is 1000 characters.");
            if (Request.Model == ImageModelName.Dalle2 && prompt.Length > 4000)
                throw new ArgumentOutOfRangeException(nameof(prompt), "The maximum character length for the prompt is 4000 characters.");
        }
        private const string ImageLabel = "image";
        private const string MaskLabel = "mask";
        private const string PromptLabel = "prompt";
        private const string NumberOfImagesLabel = "n";
        private const string SizeLabel = "size";
        private const string ResponseFormatLabel = "response_format";
        private const string UserLabel = "user";
        private MultipartFormDataContent CreateRequest(Stream image, string imageName, bool isEdit)
        {
            var content = new MultipartFormDataContent();
            if (image != null)
                content.Add(new ByteArrayContent(image.ToArray()), ImageLabel, imageName);
            if (isEdit && _mask != null)
                content.Add(new ByteArrayContent(_mask.ToArray()), MaskLabel, _maskName!);
            if (isEdit && Request.Prompt != null)
                content.Add(new StringContent(Request.Prompt), PromptLabel);
            content.Add(new StringContent(Request.NumberOfResults.ToString()), NumberOfImagesLabel);
            if (Request.Size != null)
                content.Add(new StringContent(Request.Size), SizeLabel);
            content.Add(new StringContent(Request.ResponseFormat!), ResponseFormatLabel);
            if (!string.IsNullOrWhiteSpace(Request.User))
                content.Add(new StringContent(Request.User), UserLabel);
            return content;
        }
        private const string GenerationEndpoint = "/generations";
        private const string EditEndpoint = "/edits";
        private const string VariationEndpoint = "/variations";

        public ValueTask<ImageResult> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
        {
            Request.Prompt = prompt;
            CheckPromptLength(prompt);
            return ExecuteAsync(GenerationEndpoint, Request, cancellationToken);
        }
        public ValueTask<ImageResultForBase64> GenerateAsBase64Async(string prompt, CancellationToken cancellationToken = default)
        {
            Request.Prompt = prompt;
            CheckPromptLength(prompt);
            return ExecuteAsBase64Async(GenerationEndpoint, Request, cancellationToken);
        }
        public ValueTask<ImageResult> EditAsync(string prompt, Stream file, string fileName = "image", CancellationToken cancellationToken = default)
        {
            Request.Prompt = prompt;
            CheckPromptLength(prompt);
            return ExecuteAsync(EditEndpoint, CreateRequest(file, fileName, true), cancellationToken);
        }
        public ValueTask<ImageResultForBase64> EditAsBase64Async(string prompt, Stream file, string fileName = "image", CancellationToken cancellationToken = default)
        {
            Request.Prompt = prompt;
            CheckPromptLength(prompt);
            return ExecuteAsBase64Async(EditEndpoint, CreateRequest(file, fileName, true), cancellationToken);
        }
        public ValueTask<ImageResult> VariateAsync(Stream file, string fileName = "image", CancellationToken cancellationToken = default)
            => ExecuteAsync(VariationEndpoint, CreateRequest(file, fileName, false), cancellationToken);
        public ValueTask<ImageResultForBase64> VariateAsBase64Async(Stream file, string fileName = "image", CancellationToken cancellationToken = default)
            => ExecuteAsBase64Async(VariationEndpoint, CreateRequest(file, fileName, false), cancellationToken);
        private ValueTask<ImageResult> ExecuteAsync<T>(string endpoint, T request, CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = FormatResultImage.Url.AsString();
            var uri = DefaultServices.Configuration.GetUri(OpenAiType.Image, _version, Request.Model!, endpoint, null);
            return DefaultServices.HttpClientWrapper
                    .PostAsync<ImageResult>(
                        uri,
                        request,
                        null,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }
        /// <summary>
        /// Create, Variate or Edit an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of base64 images.</returns>
        /// <exception cref="HttpRequestException"></exception>
        private ValueTask<ImageResultForBase64> ExecuteAsBase64Async<T>(string endpoint, T request, CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = FormatResultImage.B64Json.AsString();
            var uri = DefaultServices.Configuration.GetUri(OpenAiType.Image, _version, Request.Model!, endpoint, null);
            return DefaultServices.HttpClientWrapper
                    .PostAsync<ImageResultForBase64>(
                        uri,
                        request,
                        null,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }
        /// <summary>
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="maskName"></param>
        /// <returns></returns>
        public IOpenAiImage WithMask(Stream mask, string maskName = "mask.png")
        {
            _mask = mask;
            _maskName = maskName;
            return this;
        }
        /// <summary>
        /// The number of images to generate. Must be between 1 and 10.
        /// </summary>
        /// <param name="numberOfResults"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IOpenAiImage WithNumberOfResults(int numberOfResults)
        {
            if (numberOfResults > 10 || numberOfResults < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfResults), "The number of results must be between 1 and 10");
            Request.NumberOfResults = numberOfResults;
            ChangeUsage();
            return this;
        }
        /// <summary>
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public IOpenAiImage WithSize(ImageSize size)
        {
            _size = size;
            ChangeUsage();
            Request.Size = size.AsString();
            return this;
        }
        public IOpenAiImage WithQuality(ImageQuality quality)
        {
            _quality = quality;
            ChangeUsage();
            Request.Quality = quality.AsString();
            return this;
        }
        public IOpenAiImage WithStyle(ImageStyle style)
        {
            Request.Style = style.AsString();
            return this;
        }
        private void ChangeUsage()
        {
            Usages.Clear();
            Usages.Add(new OpenAiCost { Units = Request.NumberOfResults, UnitOfMeasure = UnitOfMeasure.Images, Kind = _size.AsCost(_quality) });
        }
        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids"></see>
        /// </summary>
        /// <param name="user">Unique identifier</param>
        /// <returns>Builder</returns>
        public IOpenAiImage WithUser(string user)
        {
            Request.User = user;
            return this;
        }

    }
}
