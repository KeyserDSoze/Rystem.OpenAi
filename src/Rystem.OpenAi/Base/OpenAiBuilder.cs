﻿using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    internal abstract class OpenAiBuilder<T> : IServiceForFactory
    {
        private protected readonly IFactory<DefaultServices> _factory;
        private protected readonly IFactory<OpenAiConfiguration> _configurationFactory;
#pragma warning disable CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.
        public OpenAiBuilder(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory)
        {
            _factory = factory;
            _configurationFactory = configurationFactory;
        }
#pragma warning restore CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.
        private string? _factoryName;
        private protected bool Forced { get; set; }
        private protected List<OpenAiCost> Usages { get; } = [];
        private protected DefaultServices DefaultServices => field ??= _factory.Create(_factoryName)!;
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
        private protected TRequest Request { get; }
        public OpenAiBuilder(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory) : base(factory, configurationFactory)
        {
            Request = new TRequest();
        }
        public T WithModel(TModel model)
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