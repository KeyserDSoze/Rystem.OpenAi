using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Image
{
    internal sealed class ImageVariationRequestBuilder : ImageModificationRequestBuilder<ImageVariationRequestBuilder>
    {
        public ImageVariationRequestBuilder(IFactory<DefaultServices> factory) : base(factory)
        { }
        private const string Variations = "/variations";
        private protected override string Endpoint => Variations;
    }
}
