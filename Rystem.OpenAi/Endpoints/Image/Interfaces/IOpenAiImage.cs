using System;
using System.IO;

namespace Rystem.OpenAi.Image
{
    public interface IOpenAiImage
    {
        /// <summary>
        /// Creates an image given a prompt.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns>Generation Builder</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        ImageCreateRequestBuilder Generate(string prompt);
        /// <summary>
        /// The image to use as the basis for the edit(s). Must be a valid PNG file, less than 4MB, and square.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageName"></param>
        /// <returns>Edit Builder</returns>
        ImageEditRequestBuilder Edit(string prompt, Stream image, string imageName = "image.png");
        /// <summary>
        /// The image to use as the basis for the edit(s). Must be a valid PNG file, less than 4MB, and square.
        /// Take the streamed image and transform it before sending in a correct png.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageName"></param>
        /// <returns>Edit Builder</returns>
        ImageEditRequestBuilder EditAndTrasformInPng(string prompt, Stream image, string imageName = "image.png");
        /// <summary>
        /// Creates a variation of a given image.
        /// </summary>
        /// <param name="image">The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.</param>
        /// <param name="imageName"></param>
        /// <returns>Variation Builder</returns>
        ImageVariationRequestBuilder Variate(Stream image, string imageName = "image.png");
        /// <summary>
        /// Creates a variation of a given image. Take the streamed image and transform it before sending in a correct png.
        /// </summary>
        /// <param name="image">The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.</param>
        /// <param name="imageName"></param>
        /// <returns>Variation Builder</returns>
        ImageVariationRequestBuilder VariateAndTransformInPng(Stream image, string imageName = "image.png");
    }
}
