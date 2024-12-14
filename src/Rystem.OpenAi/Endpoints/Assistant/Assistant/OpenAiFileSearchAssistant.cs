namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiFileSearchAssistant : IOpenAiFileSearchAssistant
    {
        private readonly OpenAiAssistant _assistant;
        private readonly AssistantFileSearchTool _assistantFileSearchTool;

        public OpenAiFileSearchAssistant(OpenAiAssistant assistant, AssistantFileSearchTool assistantFileSearchTool)
        {
            _assistant = assistant;
            _assistantFileSearchTool = assistantFileSearchTool;
        }
        public IOpenAiAssistant UseCustom(RankerName ranker, int scoreThreshold)
        {
            _assistantFileSearchTool.FileSearch ??= new();
            _assistantFileSearchTool.FileSearch.RankingOptions ??= new();
            _assistantFileSearchTool.FileSearch.RankingOptions.Ranker = ranker.Name;
            _assistantFileSearchTool.FileSearch.RankingOptions.ScoreThreshold = scoreThreshold;
            return _assistant;
        }

        public IOpenAiAssistant UseDefault()
        {
            return _assistant;
        }
    }
}
