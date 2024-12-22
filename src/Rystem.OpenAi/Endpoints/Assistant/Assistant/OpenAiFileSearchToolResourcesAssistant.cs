using System.Collections.Generic;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiFileSearchToolResourcesAssistant : IOpenAiFileSearchToolResourcesAssistant
    {
        private const string ChunkingStrategyTypeAsStatic = "static";
        private readonly OpenAiAssistant _openAiAssistant;
        private readonly AssistantFileSearchToolResources _assistantFileSearchToolResources;

        public OpenAiFileSearchToolResourcesAssistant(OpenAiAssistant openAiAssistant,
            AssistantFileSearchToolResources assistantFileSearchToolResources)
        {
            _openAiAssistant = openAiAssistant;
            _assistantFileSearchToolResources = assistantFileSearchToolResources;
        }
        public IOpenAiFileSearchToolResourcesAssistant AddMetadata(string key, string value)
        {
            _assistantFileSearchToolResources.Metadata ??= [];
            if (!_assistantFileSearchToolResources.Metadata.ContainsKey(key))
                _assistantFileSearchToolResources.Metadata.Add(key, value);
            else
                _assistantFileSearchToolResources.Metadata[key] = value;
            return this;
        }

        public IOpenAiFileSearchToolResourcesAssistant AddMetadata(Dictionary<string, string> metadata)
        {
            _assistantFileSearchToolResources.Metadata = metadata;
            return this;
        }

        public IOpenAiFileSearchToolResourcesAssistant ClearMetadata()
        {
            _assistantFileSearchToolResources.Metadata?.Clear();
            return this;
        }

        public IOpenAiFileSearchToolResourcesAssistant RemoveMetadata(string key)
        {
            if (_assistantFileSearchToolResources.Metadata?.ContainsKey(key) == true)
                _assistantFileSearchToolResources.Metadata.Remove(key);
            return this;
        }

        public IOpenAiAssistant Use()
        {
            return _openAiAssistant;
        }

        public IOpenAiAssistant UseFileSearch(params string[] filesId)
        {
            _assistantFileSearchToolResources.VectorStores ??= new AssistantVectorStoresFileSearchToolResources();
            if (_assistantFileSearchToolResources.VectorStores.Files == null)
                _assistantFileSearchToolResources.VectorStores.Files = [];
            _assistantFileSearchToolResources.VectorStores.Files.AddRange(filesId);
            return _openAiAssistant;
        }
        public IOpenAiFileSearchToolResourcesAssistant WithStaticChunkingStrategy(int maxChunkSize, int chunkOverlap)
        {
            _assistantFileSearchToolResources.VectorStores ??= new AssistantVectorStoresFileSearchToolResources();
            _assistantFileSearchToolResources.VectorStores.ChunkingStrategy = new AssistantChunkingStrategyVectorStoresFileSearchToolResources
            {
                Type = ChunkingStrategyTypeAsStatic,
                Static = new AssistantStaticChunkingStrategyVectorStoresFileSearchToolResources
                {
                    MaxChunkSizeTokens = maxChunkSize,
                    ChunkOverlapTokens = chunkOverlap
                }
            };
            return this;
        }
    }
}
