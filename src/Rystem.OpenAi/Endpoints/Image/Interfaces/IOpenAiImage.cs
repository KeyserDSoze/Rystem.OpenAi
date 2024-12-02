using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

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
        /// Specifies a unique user identifier for tracking and abuse detection.
        /// </summary>
        /// <param name="user">The unique identifier for the user.</param>
        /// <returns>The current instance of <see cref="IOpenAiImage"/>.</returns>
        IOpenAiImage WithUser(string user);
    }
}

