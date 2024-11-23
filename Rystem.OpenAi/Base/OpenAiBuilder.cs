using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    internal abstract class OpenAiBuilder<T, TRequest, TUsage> : IServiceForFactory
        where T : class
        where TRequest : IOpenAiRequest, new()
    {
        private readonly IFactory<DefaultServices> _factory;
        private protected TRequest Request { get; } = new TRequest();
        public OpenAiBuilder(IFactory<DefaultServices> factory)
        {
            _factory = factory;
        }
        private string? _factoryName;
        private protected bool Forced { get; set; }
        private protected List<TUsage> Usages { get; } = [];
        private protected DefaultServices DefaultServices => field ??= _factory.Create(_factoryName)!;
        public void SetFactoryName(string name)
        {
            _factoryName = name;
        }
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns> <see cref="T"/></returns>
        public T WithModel(OpenAiModelName model)
        {
            Request.Model = model;
            Forced = false;
            var entity = this as T;
            return entity!;
        }
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns> <see cref="T"/></returns>
        public T WithModel(string model)
        {
            Request.Model = model;
            Forced = true;
            var entity = this as T;
            return entity!;
        }
    }
}
