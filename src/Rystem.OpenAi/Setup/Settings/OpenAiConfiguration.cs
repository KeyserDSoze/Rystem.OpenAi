using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;

[assembly: InternalsVisibleTo("Rystem.OpenAi.Test")]
namespace Rystem.OpenAi
{
    internal delegate string OpenAiUriMaker(OpenAiType type, string? version, string? model, string appendBeforeQueryString, Dictionary<string, string>? querystringParameters);
    public sealed class OpenAiConfiguration
    {
        internal OpenAiUriMaker GetUri { get; private set; } = null!;
        internal Func<HttpClientWrapper, Task>? BeforeRequest { get; private set; }
        public bool NeedClientEnrichment => BeforeRequest != null;
        public AnyOf<string?, Enum> Name { get; }
        internal Task EnrichClientAsync(HttpClientWrapper wrapper)
        {
            if (BeforeRequest != null)
                return BeforeRequest.Invoke(wrapper);
            else
                return Task.CompletedTask;
        }
        internal OpenAiSettings Settings { get; }
        internal OpenAiConfiguration(OpenAiSettings settings, AnyOf<string?, Enum>? name)
        {
            Settings = settings;
            Name = name ?? string.Empty;
            ConfigureEndpoints();
        }
        private sealed class UriHelperConfigurator
        {
            public required string ChatUri { get; init; }
            public required string EmbeddingUri { get; init; }
            public required string FileUri { get; init; }
            public required string UploadUri { get; init; }
            public required string FineTuneUri { get; init; }
            public required string ModelUri { get; init; }
            public required string ModerationUri { get; init; }
            public required string ImageUri { get; init; }
            public required string AudioTranscriptionUri { get; init; }
            public required string AudioTranslationUri { get; init; }
            public required string AudioSpeechUri { get; init; }
            public required string BillingUri { get; init; }
            public required string DeploymentUri { get; init; }
            public required string AssistantUri { get; init; }
            public required string ThreadUri { get; init; }
            public required string VectorStoreUri { get; init; }
            public required string RealTimeUri { get; init; }
        }
        internal void ConfigureEndpoints()
        {
            var basePath = Settings.Azure.HasConfiguration ? $"{Settings.Azure.ResourceName}.openai.azure.com/openai" : "api.openai.com/{0}";
            var basePathWithModel = Settings.Azure.HasConfiguration ? $"{Settings.Azure.ResourceName}.openai.azure.com/openai/deployments/{{2}}" : "api.openai.com/{0}";
            var endPath = Settings.Azure.HasConfiguration ? "{1}?api-version={0}" : "{1}";
            var endPathWithModel = Settings.Azure.HasConfiguration ? "{1}?api-version={0}&deployment={2}" : "{1}";
            if (Settings.Azure.HasConfiguration)
            {
                SetManagedIdentityForAzure();
            }
            var uriHelper = new UriHelperConfigurator
            {
                ChatUri = $"https://{basePathWithModel}/chat/completions{endPath}",
                EmbeddingUri = $"https://{basePathWithModel}/embeddings{endPath}",
                FileUri = $"https://{basePath}/files{endPath}",
                UploadUri = $"https://{basePath}/uploads{endPath}",
                FineTuneUri = $"https://{basePath}/fine_tuning/jobs{endPath}",
                ModelUri = $"https://{basePath}/models{endPath}",
                ModerationUri = $"https://{basePath}/moderations{endPath}",
                ImageUri = $"https://{basePathWithModel}/images{endPath}",
                AudioTranscriptionUri = $"https://{basePathWithModel}/audio/transcriptions{endPath}",
                AudioTranslationUri = $"https://{basePathWithModel}/audio/translations{endPath}",
                AudioSpeechUri = $"https://{basePathWithModel}/audio/speech{endPath}",
                BillingUri = $"https://{basePath}/dashboard/billing/usage{endPath}",
                AssistantUri = $"https://{basePath}/assistants{endPath}",
                ThreadUri = $"https://{basePath}/threads{endPath}",
                VectorStoreUri = $"https://{basePath}/vector_stores{endPath}",
                DeploymentUri = $"https://{basePath}/deployments{endPath}",
                RealTimeUri = $"https://{basePath}/realtime{endPathWithModel}"
            };

            GetUri = (type, version, model, appendBeforeQueryString, querystring)
                => GetUriForOpenAi(type, version, Settings.Azure.HasConfiguration ? model ?? string.Empty : string.Empty, appendBeforeQueryString, querystring, uriHelper, Settings);
        }
        private static string GetUriForOpenAi(OpenAiType type,
           string? version,
           string? model,
           string appendBeforeQueryString,
           Dictionary<string, string>? querystring,
           UriHelperConfigurator uriHelper,
           OpenAiSettings settings)
        {
            var versionForRequest = GetVersion(settings, version, type);
            return type switch
            {
                OpenAiType.AudioTranscription => string.Format(uriHelper.AudioTranscriptionUri, versionForRequest, appendBeforeQueryString, model),
                OpenAiType.AudioTranslation => string.Format(uriHelper.AudioTranslationUri, versionForRequest, appendBeforeQueryString, model),
                OpenAiType.AudioSpeech => string.Format(uriHelper.AudioSpeechUri, versionForRequest, appendBeforeQueryString, model),
                OpenAiType.Moderation => string.Format(uriHelper.ModerationUri, versionForRequest, appendBeforeQueryString, model),
                OpenAiType.Image => string.Format(uriHelper.ImageUri, versionForRequest, appendBeforeQueryString, model),
                OpenAiType.Embedding => string.Format(uriHelper.EmbeddingUri, versionForRequest, appendBeforeQueryString, model),
                OpenAiType.Chat => string.Format(uriHelper.ChatUri, versionForRequest, appendBeforeQueryString, model),
                OpenAiType.File => string.Format(uriHelper.FileUri, versionForRequest, appendBeforeQueryString, model),
                OpenAiType.Upload => string.Format(uriHelper.UploadUri, versionForRequest, appendBeforeQueryString, model),
                OpenAiType.FineTuning => string.Format(uriHelper.FineTuneUri, versionForRequest, appendBeforeQueryString, model),
                OpenAiType.Billing => string.Format(uriHelper.BillingUri, appendBeforeQueryString, model),
                OpenAiType.Assistant => PassForQuerystringCheck(querystring, string.Format(uriHelper.AssistantUri, versionForRequest, appendBeforeQueryString, model)),
                OpenAiType.Thread => PassForQuerystringCheck(querystring, string.Format(uriHelper.ThreadUri, versionForRequest, appendBeforeQueryString, model)),
                OpenAiType.VectorStore => PassForQuerystringCheck(querystring, string.Format(uriHelper.VectorStoreUri, versionForRequest, appendBeforeQueryString, model)),
                OpenAiType.RealTime => string.Format(uriHelper.RealTimeUri, versionForRequest, appendBeforeQueryString, model),
                _ => string.Format(uriHelper.ModelUri, versionForRequest, appendBeforeQueryString, model),
            };

        }
        private const string QuestionMark = "?";
        private static string PassForQuerystringCheck(Dictionary<string, string>? querystring, string uri)
        {
            if (querystring != null && querystring.Count > 0)
            {
                var query = string.Join("&", querystring.Select(current => $"{current.Key}={current.Value}"));
                if (uri.Contains(QuestionMark))
                {
                    uri = $"{uri}&{query}";
                }
                else
                {
                    uri = $"{uri}?{query}";
                }
            }
            return uri;
        }
        private const string AuthorizationScheme = "Bearer";
        private void SetManagedIdentityForAzure()
        {
            var scopes = new[] { $"https://cognitiveservices.azure.com/.default" };
            if (Settings.Azure.HasManagedIdentity)
            {
                var credential = Settings.Azure.ManagedIdentity.UseDefault ?
                    new DefaultAzureCredential() :
                    new DefaultAzureCredential(new DefaultAzureCredentialOptions
                    {
                        ManagedIdentityClientId = Settings.Azure.ManagedIdentity.Id
                    });
                BeforeRequest = async wrapper =>
                {
                    var accessToken = await credential.GetTokenAsync(new TokenRequestContext(scopes));
                    wrapper.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, accessToken.Token);
                };
            }
            else if (Settings.Azure.HasAppRegistration)
            {
                var credential = ConfidentialClientApplicationBuilder.Create(Settings.Azure.AppRegistration.ClientId)
                    .WithClientSecret(Settings.Azure.AppRegistration.ClientSecret)
                    .WithAuthority(AadAuthorityAudience.AzureAdMyOrg, true)
                    .WithTenantId(Settings.Azure.AppRegistration.TenantId)
                    .Build();
                BeforeRequest = async wrapper =>
                {
                    var accessToken = await credential
                                        .AcquireTokenForClient(scopes)
                                        .ExecuteAsync();
                    wrapper.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, accessToken.AccessToken);
                };
            }
        }
        private const string OpenAiDefaultVersion = "v1";
        private const string AzureDefaultVersion = "2024-10-21";
        private const string AzureDefaultVersionAssistant = "2024-05-01-preview";
        private static string? GetVersion(OpenAiSettings settings, string? version, OpenAiType type)
        {
            if (version != null)
                return version;
            else if (settings.DefaultVersion != null)
                return settings.DefaultVersion;
            else if (settings.Azure.HasConfiguration)
            {
                if (type == OpenAiType.Assistant || type == OpenAiType.Thread || type == OpenAiType.VectorStore)
                    return AzureDefaultVersionAssistant;
                return AzureDefaultVersion;
            }
            return OpenAiDefaultVersion;
        }
    }
}
