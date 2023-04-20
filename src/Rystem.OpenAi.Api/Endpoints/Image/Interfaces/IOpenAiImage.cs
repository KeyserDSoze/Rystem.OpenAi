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
    [Obsolete("In version 3.x we'll remove IOpenAiImageApi and we'll use only IOpenAiImage to retrieve services")]
    public interface IOpenAiImageApi : IOpenAiImage
    {
    }
}
