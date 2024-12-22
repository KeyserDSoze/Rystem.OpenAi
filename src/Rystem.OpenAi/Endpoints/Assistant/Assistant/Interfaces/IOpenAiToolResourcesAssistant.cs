namespace Rystem.OpenAi.Assistant
{
    public interface IOpenAiToolResourcesAssistant
    {
        /// <summary>
        /// A list of file IDs made available to the code_interpreter tool. There can be a maximum of 20 files associated with the tool..
        /// </summary>
        /// <param name="filesId"></param>
        /// <returns></returns>
        IOpenAiAssistant UseCodeInterpreters(params string[] filesId);
        /// <summary>
        /// The vector store attached to this assistant. There can be a maximum of 1 vector store attached to the assistant.
        /// </summary>
        /// <param name="vectorStoresId"></param>
        /// <returns></returns>
        IOpenAiFileSearchToolResourcesAssistant WithFileSearch(params string[] vectorStoresId);
    }
}
