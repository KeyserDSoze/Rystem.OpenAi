using System.IO;
using System.Net.Http;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageVariationRequestBuilder : ImageModificationRequestBuilder<ImageVariationRequestBuilder>
    {
        internal ImageVariationRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string? prompt,
             Stream image, string imageName, bool transform, ImageSize size, IOpenAiUtility utility) :
             base(client, configuration, prompt, image, imageName, transform, size, utility)
        { }
        private const string Variations = "/variations";
        private protected override string Endpoint => Variations;
    }
}
