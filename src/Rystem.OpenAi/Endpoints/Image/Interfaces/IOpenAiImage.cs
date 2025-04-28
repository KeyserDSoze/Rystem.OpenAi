using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Image
{
    /// <summary>
    /// Interface for building and executing OpenAI image operations such as generating, editing, and variating images.
    /// </summary>
    public interface IOpenAiImage : IOpenAiBase<IOpenAiImage, ImageModelName>
    {
        /// <summary>
        /// Generates an image based on the provided prompt.
        /// </summary>
        /// <param name="prompt">The text prompt to generate the image.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and returns an <see cref="ImageResult"/>.</returns>
        ValueTask<ImageResult> GenerateAsync(string prompt, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates an image as a Base64 string based on the provided prompt.
        /// </summary>
        /// <param name="prompt">The text prompt to generate the image.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and returns an <see cref="ImageResultForBase64"/>.</returns>
        ValueTask<ImageResultForBase64> GenerateAsBase64Async(string prompt, CancellationToken cancellationToken = default);

        /// <summary>
        /// Edits an image using the provided prompt and image file.
        /// </summary>
        /// <param name="prompt">The text prompt for editing the image.</param>
        /// <param name="file">The stream containing the image file to edit.</param>
        /// <param name="fileName">The name of the image file.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and returns an <see cref="ImageResult"/>.</returns>
        ValueTask<ImageResult> EditAsync(string prompt, Stream file, string fileName = "image", CancellationToken cancellationToken = default);

        /// <summary>
        /// Edits an image and returns the result as a Base64 string.
        /// </summary>
        /// <param name="prompt">The text prompt for editing the image.</param>
        /// <param name="file">The stream containing the image file to edit.</param>
        /// <param name="fileName">The name of the image file.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and returns an <see cref="ImageResultForBase64"/>.</returns>
        ValueTask<ImageResultForBase64> EditAsBase64Async(string prompt, Stream file, string fileName = "image", CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates variations of an image based on the provided image file.
        /// </summary>
        /// <param name="file">The stream containing the image file to vary.</param>
        /// <param name="fileName">The name of the image file.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and returns an <see cref="ImageResult"/>.</returns>
        ValueTask<ImageResult> VariateAsync(Stream file, string fileName = "image", CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates variations of an image and returns the result as a Base64 string.
        /// </summary>
        /// <param name="file">The stream containing the image file to vary.</param>
        /// <param name="fileName">The name of the image file.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and returns an <see cref="ImageResultForBase64"/>.</returns>
        ValueTask<ImageResultForBase64> VariateAsBase64Async(Stream file, string fileName = "image", CancellationToken cancellationToken = default);

        /// <summary>
        /// Specifies an additional image mask for editing purposes.
        /// </summary>
        /// <param name="mask">The stream containing the mask file.</param>
        /// <param name="maskName">The name of the mask file.</param>
        /// <returns>The current instance of <see cref="IOpenAiImage"/>.</returns>
        IOpenAiImage WithMask(Stream mask, string maskName = "mask.png");

        /// <summary>
        /// Specifies the number of images to generate.
        /// </summary>
        /// <param name="numberOfResults">The number of images to generate (1 to 10).</param>
        /// <returns>The current instance of <see cref="IOpenAiImage"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the number of results is out of range.</exception>
        IOpenAiImage WithNumberOfResults(int numberOfResults);

        /// <summary>
        /// Specifies the size of the generated images.
        /// </summary>
        /// <param name="size">The desired size of the images (e.g., 256x256, 512x512, 1024x1024).</param>
        /// <returns>The current instance of <see cref="IOpenAiImage"/>.</returns>
        IOpenAiImage WithSize(ImageSize size);

        /// <summary>
        /// Specifies the quality of the generated images.
        /// </summary>
        /// <param name="quality">The desired quality of the images.</param>
        /// <returns>The current instance of <see cref="IOpenAiImage"/>.</returns>
        IOpenAiImage WithQuality(ImageQuality quality);

        /// <summary>
        /// Specifies the style of the generated images.
        /// </summary>
        /// <param name="style">The desired style of the images.</param>
        /// <returns>The current instance of <see cref="IOpenAiImage"/>.</returns>
        IOpenAiImage WithStyle(ImageStyle style);
        /// <summary>
        /// The format in which the generated images are returned. This parameter is only supported for gpt-image-1. Must be one of png, jpeg, or webp.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        IOpenAiImage WithOutputFormat(ImageOutputFormat format);
        /// <summary>
        /// The compression level (0-100%) for the generated images. This parameter is only supported for gpt-image-1 with the webp or jpeg output formats, and defaults to 100.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IOpenAiImage WithOutputCompression(int? value);
        /// <summary>
        /// Control the content-moderation level for images generated by gpt-image-1. Must be either low for less restrictive filtering or auto (default value).
        /// </summary>
        /// <returns></returns>
        IOpenAiImage WithLowModeration();
        /// <summary>
        /// Control the content-moderation level for images generated by gpt-image-1. Must be either low for less restrictive filtering or auto (default value).
        /// </summary>
        /// <returns></returns>
        IOpenAiImage WithDefaultModeration();
        /// <summary>
        /// Allows to set transparency for the background of the generated image(s). This parameter is only supported for gpt-image-1. Must be one of transparent, opaque or auto (default value). When auto is used, the model will automatically determine the best background for the image.
        /// If transparent, the output format needs to support transparency, so it should be set to either png(default value) or webp.
        /// </summary>
        /// <returns></returns>
        IOpenAiImage WithBackground(ImageBackgroundFormat format);
        /// <summary>
        /// Specifies a unique user identifier for tracking and abuse detection.
        /// </summary>
        /// <param name="user">The unique identifier for the user.</param>
        /// <returns>The current instance of <see cref="IOpenAiImage"/>.</returns>
        IOpenAiImage WithUser(string user);
    }
}

