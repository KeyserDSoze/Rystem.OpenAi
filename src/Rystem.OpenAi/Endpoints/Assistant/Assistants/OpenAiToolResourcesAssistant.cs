namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiToolResourcesAssistant : IOpenAiToolResourcesAssistant
    {
        private readonly OpenAiAssistant _openAiAssistant;
        public OpenAiToolResourcesAssistant(OpenAiAssistant openAiAssistant)
        {
            _openAiAssistant = openAiAssistant;
        }
        public IOpenAiAssistant UseCodeInterpreters(params string[] filesId)
        {
            _openAiAssistant.Request.ToolResources ??= new AssistantToolResources
            {
            };
            _openAiAssistant.Request.ToolResources.CodeInterpreter = new AssistantCodeInterpreterToolResources
            {
                Files = [.. filesId]
            };
            return _openAiAssistant;
        }

        public IOpenAiFileSearchToolResourcesAssistant WithFileSearch(params string[] vectorStoresId)
        {
            _openAiAssistant.Request.ToolResources ??= new AssistantToolResources
            {
            };
            var assistantFileSearchToolResources = new AssistantFileSearchToolResources
            {
                VectorStoresId = [.. vectorStoresId]
            };
            _openAiAssistant.Request.ToolResources.FileSearch = assistantFileSearchToolResources;
            return new OpenAiFileSearchToolResourcesAssistant(_openAiAssistant, assistantFileSearchToolResources);
        }
    }
}
