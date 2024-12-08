using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Rystem.OpenAi
{
    public sealed class OpenAiSettings : IFactoryOptions
    {
        public string? ApiKey { get; set; }
        public string? OrganizationName { get; set; }
        public string? ProjectId { get; set; }
        public string? Version { get; set; }
        public DefaultRequestConfiguration DefaultRequestConfiguration { get; set; } = new();
        private OpenAiAzureSettings? _azureSettings;
        public OpenAiAzureSettings Azure => _azureSettings ??= new OpenAiAzureSettings();
        /// <summary>
        /// If you not set a retry policy, the dafault value is 3 retries every 0.5 seconds.
        /// </summary>
        public bool RetryPolicy { get; set; } = true;
        /// <summary>
        /// Sample with 3 retries every 0.5 seconds:
        /// Policy<HttpResponseMessage>
        ///.Handle<HttpRequestException>()
        ///.OrTransientHttpError()
        ///.WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(0.5));
        /// </summary>
        public IAsyncPolicy<HttpResponseMessage>? CustomRetryPolicy { get; set; }
        /// <summary>
        /// 50% of requests fail in 10 seconds time window with a minimum of 10 requests in 15 seconds.
        /// </summary>
        /// <returns></returns>
        public OpenAiSettings SetCircuitBreakerDefaultRetryPolicy()
        {
            RetryPolicy = true;
            CustomRetryPolicy = Policy<HttpResponseMessage>
                   .Handle<HttpRequestException>()
                   .OrTransientHttpError()
                   .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromSeconds(10), 10, TimeSpan.FromSeconds(15));
            return this;
        }
        internal const string HttpClientName = "openaiclient_rystem";
        internal Dictionary<OpenAiType, string> Versions { get; } = [];
        public OpenAiSettings UseVersionFor(OpenAiType type, string version)
        {
            if (!Versions.TryAdd(type, version))
                Versions[type] = version;
            return this;
        }
        public OpenAiSettings UseVersionForChat(string version)
            => UseVersionFor(OpenAiType.Chat, version);
        public OpenAiSettings UseVersionForFineTune(string version)
            => UseVersionFor(OpenAiType.FineTuning, version);
        public OpenAiSettings UseVersionForFile(string version)
            => UseVersionFor(OpenAiType.File, version);
        public OpenAiSettings UseVersionForModel(string version)
            => UseVersionFor(OpenAiType.Model, version);
        public OpenAiSettings UseVersionForAudioTranslation(string version)
            => UseVersionFor(OpenAiType.AudioTranslation, version);
        public OpenAiSettings UseVersionForAudioSpeech(string version)
         => UseVersionFor(OpenAiType.AudioSpeech, version);
        public OpenAiSettings UseVersionForAudioTranscription(string version)
            => UseVersionFor(OpenAiType.AudioTranscription, version);
        public OpenAiSettings UseVersionForImage(string version)
            => UseVersionFor(OpenAiType.Image, version);
        public OpenAiSettings UseVersionForModeration(string version)
            => UseVersionFor(OpenAiType.Moderation, version);
        public OpenAiSettings UseVersionForEmbedding(string version)
            => UseVersionFor(OpenAiType.Embedding, version);
        public PriceBuilder PriceBuilder { get; } = PriceBuilder.Default;
        internal Dictionary<OpenAiType, Dictionary<string, ModelName>> Deployments { get; } = [];
        internal Dictionary<OpenAiType, Dictionary<string, ModelName>> ModelDeployments { get; } = [];
        /// <summary>
        /// Map a deployment name for a type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="newName"></param>
        /// <param name="officialName"></param>
        /// <returns></returns>
        public OpenAiSettings MapDeployment(OpenAiType type, string newName, ModelName officialName)
        {
            if (!Deployments.ContainsKey(type))
                Deployments.Add(type, []);
            if (!ModelDeployments.ContainsKey(type))
                ModelDeployments.Add(type, []);
            Deployments[type].TryAdd(newName, officialName);
            ModelDeployments[type].TryAdd(officialName, newName);
            return this;
        }
        /// <summary>
        /// Map a deployment name for a type, overwrite every model you use in the application for that type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public OpenAiSettings MapDeploymentForEveryRequests(OpenAiType type, string newName)
        {
            if (!Deployments.ContainsKey(type))
                Deployments.Add(type, []);
            if (!ModelDeployments.ContainsKey(type))
                ModelDeployments.Add(type, []);
            Deployments[type].TryAdd(newName, OpenAiConfiguration.Asterisk);
            ModelDeployments[type].TryAdd(OpenAiConfiguration.Asterisk, newName);
            return this;
        }
    }
}
