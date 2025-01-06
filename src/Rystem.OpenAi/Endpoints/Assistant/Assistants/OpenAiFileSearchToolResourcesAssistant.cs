using System;
using System.Collections.Generic;
using System.Linq;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiFileSearchToolResourcesAssistant<T> : IOpenAiFileSearchToolResourcesAssistant<T>
    {
        private const string ChunkingStrategyTypeAsStatic = "static";
        private readonly T _builder;
        private readonly AssistantFileSearchToolResources _assistantFileSearchToolResources;

        public OpenAiFileSearchToolResourcesAssistant(T builder,
            AssistantFileSearchToolResources assistantFileSearchToolResources)
        {
            _builder = builder;
            _assistantFileSearchToolResources = assistantFileSearchToolResources;
        }
        public IOpenAiFileSearchToolResourcesAssistant<T> AddMetadata(string key, string value)
        {
            var assistantVectorStoresFileSearchTool = CreateOrGet();
            assistantVectorStoresFileSearchTool.Metadata ??= [];
            if (!assistantVectorStoresFileSearchTool.Metadata.TryAdd(key, value))
                assistantVectorStoresFileSearchTool.Metadata[key] = value;
            return this;
        }

        public IOpenAiFileSearchToolResourcesAssistant<T> AddMetadata(Dictionary<string, string> metadata)
        {
            var assistantVectorStoresFileSearchTool = CreateOrGet();
            assistantVectorStoresFileSearchTool.Metadata = metadata;
            return this;
        }

        public IOpenAiFileSearchToolResourcesAssistant<T> ClearMetadata()
        {
            var assistantVectorStoresFileSearchTool = CreateOrGet();
            assistantVectorStoresFileSearchTool.Metadata?.Clear();
            return this;
        }

        public IOpenAiFileSearchToolResourcesAssistant<T> RemoveMetadata(string key)
        {
            var assistantVectorStoresFileSearchTool = CreateOrGet();
            if (assistantVectorStoresFileSearchTool.Metadata?.ContainsKey(key) == true)
                assistantVectorStoresFileSearchTool.Metadata.Remove(key);
            return this;
        }
        public T Use(params string[] filesId)
        {
            var assistantVectorStoresFileSearchTool = CreateOrGet();
            assistantVectorStoresFileSearchTool.Files ??= [];
            assistantVectorStoresFileSearchTool.Files.AddRange(filesId);
            return _builder;
        }
        public IOpenAiFileSearchToolResourcesAssistant<T> WithStaticChunkingStrategy(int maxChunkSize, int chunkOverlap)
        {
            var assistantVectorStoresFileSearchTool = CreateOrGet();
            assistantVectorStoresFileSearchTool.ChunkingStrategy = new AssistantChunkingStrategyVectorStoresFileSearchToolResources
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
        private AssistantVectorStoresFileSearchToolResources CreateOrGet()
        {
            _assistantFileSearchToolResources.VectorStores ??= [];
            var assistantVectorStoresFileSearchTool = _assistantFileSearchToolResources.VectorStores.FirstOrDefault() ?? new AssistantVectorStoresFileSearchToolResources
            {
            };
            if (_assistantFileSearchToolResources.VectorStores.Count == 0)
                _assistantFileSearchToolResources.VectorStores.Add(assistantVectorStoresFileSearchTool);
            return assistantVectorStoresFileSearchTool;
        }
    }
}
