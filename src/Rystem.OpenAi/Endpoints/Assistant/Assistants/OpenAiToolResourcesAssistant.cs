using System;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiToolResourcesAssistant<T> : IOpenAiToolResourcesAssistant<T>
    {
        private readonly T _builder;
        private readonly AssistantToolResources _toolResources;

        public OpenAiToolResourcesAssistant(T builder, AssistantToolResources toolResources)
        {
            _builder = builder;
            _toolResources = toolResources;
        }
        public T UseCodeInterpreters(params string[] filesId)
        {
            _toolResources.CodeInterpreter = new AssistantCodeInterpreterToolResources
            {
                Files = [.. filesId]
            };
            return _builder;
        }

        public IOpenAiFileSearchToolResourcesAssistant<T> WithFileSearch(params string[] vectorStoresId)
        {
            var assistantFileSearchToolResources = new AssistantFileSearchToolResources
            {
                VectorStoresId = [.. vectorStoresId]
            };
            _toolResources.FileSearch = assistantFileSearchToolResources;
            return new OpenAiFileSearchToolResourcesAssistant<T>(_builder, assistantFileSearchToolResources);
        }
    }
}
