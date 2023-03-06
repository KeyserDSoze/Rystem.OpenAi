using System.IO;

namespace Rystem.OpenAi.Image
{
    public interface IOpenAiImageApi
    {
        ImageCreateRequestBuilder Generate(string prompt);
        ImageVariationRequestBuilder Variate(Stream image, string imageName = "image.png");
    }
}
