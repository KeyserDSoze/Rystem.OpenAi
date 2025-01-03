using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi
{
    internal abstract class OpenAiBuilderWithMetadata<T, TRequest, TModel> : OpenAiBuilder<T, TRequest, TModel>, IServiceForFactory
        where T : class
        where TRequest : IOpenAiRequestWithMetadata, new()
        where TModel : ModelName
    {
        public OpenAiBuilderWithMetadata(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, params OpenAiType[] types)
            : base(factory, configurationFactory, types)
        {
        }
        public T AddMetadata(string key, string value)
        {
            Request.Metadata ??= [];
            Request.Metadata.TryAdd(key, value);
            var entity = this as T;
            return entity!;
        }
        public T AddMetadata(Dictionary<string, string> metadata)
        {
            Request.Metadata = metadata;
            var entity = this as T;
            return entity!;
        }
        public T ClearMetadata()
        {
            Request.Metadata?.Clear();
            var entity = this as T;
            return entity!;
        }
        public T RemoveMetadata(string key)
        {
            if (Request.Metadata?.ContainsKey(key) == true)
                Request.Metadata.Remove(key);
            var entity = this as T;
            return entity!;
        }
    }
}
