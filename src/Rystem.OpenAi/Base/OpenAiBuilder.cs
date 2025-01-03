using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    internal abstract class OpenAiBuilder<T> : IServiceForFactory
    {
        private protected readonly IFactory<DefaultServices> Factory;
        private protected readonly IFactory<OpenAiConfiguration> ConfigurationFactory;
        private protected readonly OpenAiType[] Types;
#pragma warning disable CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.
        public OpenAiBuilder(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, params OpenAiType[] types)
        {
            Factory = factory;
            ConfigurationFactory = configurationFactory;
            Types = types;
        }
#pragma warning restore CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.
        private string? _factoryName;
        private protected bool Forced { get; set; }
        private protected List<OpenAiCost> Usages { get; } = [];
        internal DefaultServices DefaultServices => field ??= Factory.Create(_factoryName)!;
        internal OpenAiConfiguration OpenAiConfiguration => field ??= ConfigurationFactory.Create(_factoryName)!;
        public void SetFactoryName(string name)
        {
            _factoryName = name;
            ConfigureFactory(name);
        }
        private protected abstract void ConfigureFactory(string name);
    }
    internal abstract class OpenAiBuilder<T, TRequest, TModel> : OpenAiBuilder<T>, IServiceForFactory
        where T : class
        where TRequest : IOpenAiRequest, new()
        where TModel : ModelName
    {
        internal TRequest Request { get; }
        public OpenAiBuilder(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, params OpenAiType[] types)
            : base(factory, configurationFactory, types)
        {
            Request = new TRequest();
        }
        public T WithModel(TModel model)
        {
            var modelName = model.Name;
            foreach (var type in Types)
            {
                if (OpenAiConfiguration.Settings.ModelDeployments.ContainsKey(type))
                {
                    if (OpenAiConfiguration.Settings.ModelDeployments[type].TryGetValue(model, out var value))
                    {
                        modelName = value.Name;
                    }
                    else if (OpenAiConfiguration.Settings.ModelDeployments[type].TryGetValue(OpenAiConfiguration.Asterisk, out var asteriskValue))
                    {
                        modelName = asteriskValue.Name;
                    }
                }
            }
            Request.Model = modelName;
            Forced = false;
            var entity = this as T;
            return entity!;
        }
        public T ForceModel(string model)
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
