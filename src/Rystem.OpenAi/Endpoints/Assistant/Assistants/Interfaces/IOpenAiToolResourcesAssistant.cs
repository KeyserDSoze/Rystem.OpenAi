namespace Rystem.OpenAi.Assistant
{
    public interface IOpenAiToolResourcesAssistant<T>
    {
        /// <summary>
        /// A list of file IDs made available to the code_interpreter tool. There can be a maximum of 20 files associated with the tool..
        /// </summary>
        /// <param name="filesId"></param>
        /// <returns></returns>
        T UseCodeInterpreters(params string[] filesId);
        /// <summary>
        /// Create a new vector store with some files and a chunking strategy.
        /// </summary>
        /// <returns></returns>
        IOpenAiFileSearchToolResourcesAssistant<T> WithFileSearch();
        /// <summary>
        /// Add a already created vector store.
        /// </summary>
        /// <param name="vectorStoresId"></param>
        /// <returns></returns>
        T WithVectorStoresAsFileSearch(params string[] vectorStoresId);
    }
}
