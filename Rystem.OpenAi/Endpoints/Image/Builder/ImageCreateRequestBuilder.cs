using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Image
{
    internal sealed class ImageCreateRequestBuilder : ImageRequestBuilder<ImageCreateRequestBuilder>
    {
        public ImageCreateRequestBuilder(IFactory<DefaultServices> factory) : base(factory)
        {
        }
        private const string Generations = "/generations";
        private protected override string Endpoint => Generations;
        private protected override object CreateRequest() => Request;
    }
}
