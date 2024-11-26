using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    internal abstract class OpenAiBuilder<T> : IServiceForFactory
    {
        private protected readonly IFactory<DefaultServices> _factory;
        public OpenAiBuilder(IFactory<DefaultServices> factory)
        {
            _factory = factory;
        }
        private string? _factoryName;
        private protected bool Forced { get; set; }
        private protected List<OpenAiCost> Usages { get; } = [];
        private protected DefaultServices DefaultServices => field ??= _factory.Create(_factoryName)!;
        public void SetFactoryName(string name)
        {
            _factoryName = name;
        }
    }
    internal abstract class OpenAiBuilder<T, TRequest> : OpenAiBuilder<T>, IServiceForFactory
        where T : class
        where TRequest : IOpenAiRequest, new()
    {
        private protected TRequest Request { get; }
        public OpenAiBuilder(IFactory<DefaultServices> factory) : base(factory)
        {
            Request = new TRequest();
        }
        public T WithModel(ModelName model)
        {
            Request.Model = model;
            Forced = false;
            var entity = this as T;
            return entity!;
        }
        public T WithModel(string model)
        {
            Request.Model = model;
            Forced = true;
            var entity = this as T;
            return entity!;
        }
        public decimal CalculateCost()
        {
            var outputPrice = DefaultServices.Price.CalculatePrice(Request.Model!, [.. Usages]);
            return outputPrice;
        }
    }
}
