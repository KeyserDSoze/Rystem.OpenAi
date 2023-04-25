using System;
using System.IO;

namespace Rystem.OpenAi.Image
{
    public interface IOpenAiImage
    {
        ImageCreateRequestBuilder Generate(string prompt);
        ImageVariationRequestBuilder Variate(Stream image, string imageName = "image.png");
        ImageVariationRequestBuilder VariateAndTransformInPng(Stream image, string imageName = "image.png");
    }
}
