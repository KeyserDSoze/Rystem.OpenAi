using System;
using System.Collections.Generic;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiFileSearchToolResourcesAssistant<T> : IOpenAiFileSearchToolResourcesAssistant<T>
    {
        private const string ChunkingStrategyTypeAsStatic = "static";
        private readonly AnyOf<OpenAiAssistant, OpenAiThread> _openAiAssistant;
        private readonly AssistantFileSearchToolResources _assistantFileSearchToolResources;

        public OpenAiFileSearchToolResourcesAssistant(AnyOf<OpenAiAssistant, OpenAiThread> openAiAssistant,
            AssistantFileSearchToolResources assistantFileSearchToolResources)
        {
            _openAiAssistant = openAiAssistant;
            _assistantFileSearchToolResources = assistantFileSearchToolResources;
        }
        public IOpenAiFileSearchToolResourcesAssistant<T> AddMetadata(string key, string value)
        {
            _assistantFileSearchToolResources.Metadata ??= [];
            if (!_assistantFileSearchToolResources.Metadata.TryAdd(key, value))
                _assistantFileSearchToolResources.Metadata[key] = value;
            return this;
        }

        public IOpenAiFileSearchToolResourcesAssistant<T> AddMetadata(Dictionary<string, string> metadata)
        {
            _assistantFileSearchToolResources.Metadata = metadata;
            return this;
        }

        public IOpenAiFileSearchToolResourcesAssistant<T> ClearMetadata()
        {
            _assistantFileSearchToolResources.Metadata?.Clear();
            return this;
        }

        public IOpenAiFileSearchToolResourcesAssistant<T> RemoveMetadata(string key)
        {
            if (_assistantFileSearchToolResources.Metadata?.ContainsKey(key) == true)
                _assistantFileSearchToolResources.Metadata.Remove(key);
            return this;
        }

        public T Use()
        {
            return _openAiAssistant.Get<T>()!;
        }

        public T UseFileSearch(params string[] filesId)
        {
            _assistantFileSearchToolResources.VectorStores ??= new AssistantVectorStoresFileSearchToolResources();
            if (_assistantFileSearchToolResources.VectorStores.Files == null)
                _assistantFileSearchToolResources.VectorStores.Files = [];
            _assistantFileSearchToolResources.VectorStores.Files.AddRange(filesId);
            return _openAiAssistant.Get<T>()!;
        }
        public IOpenAiFileSearchToolResourcesAssistant<T> WithStaticChunkingStrategy(int maxChunkSize, int chunkOverlap)
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
