using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    internal abstract class OpenAiBuilder<T> : IServiceForFactory
        where T : class
    {
        private protected readonly IFactory<DefaultServices> Factory;
        private protected readonly IFactory<OpenAiConfiguration> ConfigurationFactory;
        private protected readonly IOpenAiLoggerFactory LoggerFactory;
        private protected readonly OpenAiType[] Types;
        private protected string? _version;
#pragma warning disable CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.
        public OpenAiBuilder(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLoggerFactory loggerFactory, params OpenAiType[] types)
        {
            Factory = factory;
            ConfigurationFactory = configurationFactory;
            LoggerFactory = loggerFactory
                .ConfigureTypes(types);
            Types = types;
        }
#pragma warning restore CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.
        private string? _factoryName;
        private protected List<OpenAiCost> Usages { get; } = [];
        internal DefaultServices DefaultServices => field ??= Factory.Create(_factoryName)!;
        internal OpenAiConfiguration OpenAiConfiguration => field ??= ConfigurationFactory.Create(_factoryName)!;
        public void SetFactoryName(string name)
        {
            _factoryName = name;
            ConfigureFactory(name);
        }
        private protected abstract void ConfigureFactory(string name);
        public T WithVersion(string version)
        {
            _version = version;
            var entity = this as T;
            return entity!;
        }
    }
    internal abstract class OpenAiBuilder<T, TRequest, TModel> : OpenAiBuilder<T>, IServiceForFactory
        where T : class
        where TRequest : IOpenAiRequest, new()
        where TModel : ModelName
    {
        internal TRequest Request { get; }
        public OpenAiBuilder(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLoggerFactory loggerFactory, params OpenAiType[] types)
            : base(factory, configurationFactory, loggerFactory, types)
        {
            Request = new TRequest();
        }
        public T WithModel(TModel model)
        {
            var modelName = model.Name;
            Request.Model = modelName;
            var entity = this as T;
            return entity!;
        }
        public T ForceModel(string model)
        {
            Request.Model = model;
            var entity = this as T;
            return entity!;
        }
        public decimal CalculateCost()
        {
            var outputPrice = DefaultServices.Price.CalculatePrice(Request.Model!, [.. Usages]);
            return outputPrice;
        }
        public string? ModelName => Request.Model;
    }
}
