using System;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiToolResourcesAssistant<T> : IOpenAiToolResourcesAssistant<T>
    {
        private readonly AnyOf<OpenAiAssistant, OpenAiThread> _openAiAssistant;
        public OpenAiToolResourcesAssistant(AnyOf<OpenAiAssistant, OpenAiThread> openAiAssistant)
        {
            _openAiAssistant = openAiAssistant;
        }
        public T UseCodeInterpreters(params string[] filesId)
        {
            if (_openAiAssistant.Is<OpenAiAssistant>(out var assistant))
            {
                assistant!.Request.ToolResources ??= new AssistantToolResources
                {
                };
                assistant.Request.ToolResources.CodeInterpreter = new AssistantCodeInterpreterToolResources
                {
                    Files = [.. filesId]
                };
            }
            else if (_openAiAssistant.Is<OpenAiThread>(out var thread))
            {
                thread!.Request.ToolResources ??= new AssistantToolResources
                {
                };
                thread.Request.ToolResources.CodeInterpreter = new AssistantCodeInterpreterToolResources
                {
                    Files = [.. filesId]
                };
            }
            return _openAiAssistant.Get<T>()!;
        }

        public IOpenAiFileSearchToolResourcesAssistant<T> WithFileSearch(params string[] vectorStoresId)
        {
            var assistantFileSearchToolResources = new AssistantFileSearchToolResources
            {
                VectorStoresId = [.. vectorStoresId]
            };
            if (_openAiAssistant.Is<OpenAiAssistant>(out var assistant))
            {
                assistant!.Request.ToolResources ??= new AssistantToolResources();
                assistant.Request.ToolResources.FileSearch = assistantFileSearchToolResources;
            }
            else if (_openAiAssistant.Is<OpenAiThread>(out var thread))
            {
                thread!.Request.ToolResources ??= new AssistantToolResources();
                thread.Request.ToolResources.FileSearch = assistantFileSearchToolResources;
            }
            return new OpenAiFileSearchToolResourcesAssistant<T>(_openAiAssistant, assistantFileSearchToolResources);
        }
    }
}
