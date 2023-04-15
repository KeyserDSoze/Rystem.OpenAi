using System.Collections.Generic;
using System.Net.Http;
using Polly;

namespace Rystem.OpenAi
{
    public sealed class OpenAiSettings
    {
        public string? ApiKey { get; set; }
        public string? OrganizationName { get; set; }
        public string? Version { get; set; }
        public OpenAiAzureSettings Azure { get; } = new OpenAiAzureSettings();
        public OpenAiCostSettings Price { get; } = new OpenAiCostSettings();
        public bool RetryPolicy { get; set; } = true;
        public IAsyncPolicy<HttpResponseMessage>? CustomRetryPolicy { get; set; }
        internal const string HttpClientName = "openaiclient_rystem";
        internal Dictionary<OpenAiType, string> Versions { get; } = new Dictionary<OpenAiType, string>();
        public OpenAiSettings UseVersionFor(OpenAiType type, string version)
        {
            if (Versions.ContainsKey(type))
                Versions[type] = version;
            else
                Versions.Add(type, version);
            return this;
        }
        public OpenAiSettings UseVersionForChat(string version)
            => UseVersionFor(OpenAiType.Chat, version);
        public OpenAiSettings UseVersionForFineTune(string version)
            => UseVersionFor(OpenAiType.FineTune, version);
        public OpenAiSettings UseVersionForFile(string version)
            => UseVersionFor(OpenAiType.File, version);
        public OpenAiSettings UseVersionForModel(string version)
            => UseVersionFor(OpenAiType.Model, version);
        public OpenAiSettings UseVersionForAudioTranslation(string version)
            => UseVersionFor(OpenAiType.AudioTranslation, version);
        public OpenAiSettings UseVersionForAudioTranscription(string version)
            => UseVersionFor(OpenAiType.AudioTranscription, version);
        public OpenAiSettings UseVersionForCompletion(string version)
            => UseVersionFor(OpenAiType.Completion, version);
        public OpenAiSettings UseVersionForEdit(string version)
            => UseVersionFor(OpenAiType.Edit, version);
        public OpenAiSettings UseVersionForImage(string version)
            => UseVersionFor(OpenAiType.Image, version);
        public OpenAiSettings UseVersionForModeration(string version)
            => UseVersionFor(OpenAiType.Moderation, version);
        public OpenAiSettings UseVersionForEmbedding(string version)
            => UseVersionFor(OpenAiType.Embedding, version);
    }
}
