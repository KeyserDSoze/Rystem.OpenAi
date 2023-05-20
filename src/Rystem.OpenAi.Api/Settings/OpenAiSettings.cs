using System;
using System.Collections.Generic;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;

namespace Rystem.OpenAi
{
    public sealed class OpenAiSettings
    {
        public string? ApiKey { get; set; }
        public string? OrganizationName { get; set; }
        public string? Version { get; set; }
        public OpenAiAzureSettings Azure { get; } = new OpenAiAzureSettings();
        public OpenAiPriceSettings Price { get; } = new OpenAiPriceSettings();
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
