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
            _openAiAssistant.Request.ToolResources = new AssistantCodeInterpreterToolResources
            {
                Files = [.. filesId]
            };
            return _openAiAssistant;
        }

        public IOpenAiFileSearchToolResourcesAssistant WithFileSearch(params string[] vectorStoresId)
        {
            var assistantFileSearchToolResources = new AssistantFileSearchToolResources
            {
                VectorStoresId = [.. vectorStoresId]
            };
            _openAiAssistant.Request.ToolResources = assistantFileSearchToolResources;
            return new OpenAiFileSearchToolResourcesAssistant(_openAiAssistant, assistantFileSearchToolResources);
        }
    }
}
