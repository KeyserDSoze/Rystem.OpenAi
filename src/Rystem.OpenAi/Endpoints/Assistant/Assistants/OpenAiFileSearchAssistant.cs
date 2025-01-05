namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiFileSearchAssistant<T> : IOpenAiFileSearchAssistant<T>
    {
        private readonly T _builder;
        private readonly AssistantFileSearchTool _assistantFileSearchTool;

        public OpenAiFileSearchAssistant(T builder, AssistantFileSearchTool assistantFileSearchTool)
        {
            _builder = builder;
            _assistantFileSearchTool = assistantFileSearchTool;
        }
        public T UseCustom(RankerName ranker, float scoreThreshold)
        {
            _assistantFileSearchTool.FileSearch ??= new();
            _assistantFileSearchTool.FileSearch.RankingOptions ??= new();
            _assistantFileSearchTool.FileSearch.RankingOptions.Ranker = ranker.Name;
            _assistantFileSearchTool.FileSearch.RankingOptions.ScoreThreshold = scoreThreshold;
            return _builder;
        }

        public T UseDefault()
        {
            return _builder;
        }
    }
}
