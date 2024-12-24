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
    internal delegate string OpenAiUriMaker(OpenAiType type, string modelId, bool forced, string appendBeforeQueryString, Dictionary<string, string>? querystringParameters);
    public sealed class OpenAiConfiguration
    {
        private const string Forced = nameof(Forced);
        internal OpenAiUriMaker GetUri { get; private set; } = null!;
        internal Func<HttpClientWrapper, Task>? BeforeRequest { get; private set; }
        public bool NeedClientEnrichment => BeforeRequest != null;
        public bool WithAzure { get; private set; }
        public string Name { get; }
        internal Task EnrichClientAsync(HttpClientWrapper wrapper)
        {
            if (BeforeRequest != null)
                return BeforeRequest.Invoke(wrapper);
            else
                return Task.CompletedTask;
        }
        internal OpenAiSettings Settings { get; }
        internal OpenAiConfiguration(OpenAiSettings settings, string? name)
        {
            Settings = settings;
            Name = name ?? string.Empty;
            ConfigureEndpoints();
        }
        private sealed class UriHelperConfigurator
        {
            private const string BaseUri = "{0}/{1}{2}{3}";
            public string ChatUri { get; set; } = string.Format(BaseUri, "{0}", "chat/completions", "{1}", "{2}");
            public string EmbeddingUri { get; set; } = string.Format(BaseUri, "{0}", "embeddings", "{1}", "{2}");
            public string FileUri { get; set; } = string.Format(BaseUri, "{0}", "files", "{1}", "{2}");
            public string UploadUri { get; set; } = string.Format(BaseUri, "{0}", "uploads", "{1}", "{2}");
            public string FineTuneUri { get; set; } = string.Format(BaseUri, "{0}", "fine_tuning/jobs", "{1}", "{2}");
            public string ModelUri { get; set; } = string.Format(BaseUri, "{0}", "models", "{1}", "{2}");
            public string ModerationUri { get; set; } = string.Format(BaseUri, "{0}", "moderations", "{1}", "{2}");
            public string ImageUri { get; set; } = string.Format(BaseUri, "{0}", "images", "{1}", "{2}");
            public string AudioTranscriptionUri { get; set; } = string.Format(BaseUri, "{0}", "audio/transcriptions", "{1}", "{2}");
            public string AudioTranslationUri { get; set; } = string.Format(BaseUri, "{0}", "audio/translations", "{1}", "{2}");
            public string AudioSpeechUri { get; set; } = string.Format(BaseUri, "{0}", "audio/speech", "{1}", "{2}");
            public string BillingUri { get; set; } = string.Format(BaseUri, "{0}", "dashboard/billing/usage", "{1}", "{2}");
            public string DeploymentUri { get; set; } = string.Format(BaseUri, "{0}", "deployments", "{1}", "{2}");
            public string AssistantUri { get; set; } = string.Format(BaseUri, "{0}", "assistants", "{1}", "{2}");
        }
        internal void ConfigureEndpoints()
        {
            if (Settings.Azure.HasConfiguration)
                ConfigureEndpointsForAzure();
            else
                ConfigureEndpointsForOpenAi();
        }
        private void ConfigureEndpointsForOpenAi()
        {
            var uriHelper = new UriHelperConfigurator();
            Settings.Version ??= "v1";
            uriHelper.ChatUri = string.Format(uriHelper.ChatUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.Chat)}", "{0}", string.Empty);
            uriHelper.EmbeddingUri = string.Format(uriHelper.EmbeddingUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.Embedding)}", "{0}", string.Empty);
            uriHelper.FileUri = string.Format(uriHelper.FileUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.File)}", "{0}", string.Empty);
            uriHelper.UploadUri = string.Format(uriHelper.UploadUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.Upload)}", "{0}", string.Empty);
            uriHelper.FineTuneUri = string.Format(uriHelper.FineTuneUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.FineTuning)}", "{0}", string.Empty);
            uriHelper.ModelUri = string.Format(uriHelper.ModelUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.Model)}", "{0}", string.Empty);
            uriHelper.ModerationUri = string.Format(uriHelper.ModerationUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.Moderation)}", "{0}", string.Empty);
            uriHelper.ImageUri = string.Format(uriHelper.ImageUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.Image)}", "{0}", string.Empty);
            uriHelper.AudioTranscriptionUri = string.Format(uriHelper.AudioTranscriptionUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.AudioTranscription)}", "{0}", string.Empty);
            uriHelper.AudioTranslationUri = string.Format(uriHelper.AudioTranslationUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.AudioTranslation)}", "{0}", string.Empty);
            uriHelper.AudioSpeechUri = string.Format(uriHelper.AudioSpeechUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.AudioSpeech)}", "{0}", string.Empty);
            uriHelper.BillingUri = string.Format(uriHelper.BillingUri, $"https://api.openai.com", "{0}", string.Empty);
            uriHelper.AssistantUri = string.Format(uriHelper.AssistantUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.Assistant)}", "{0}", string.Empty);

            GetUri = (type, modelId, forceModel, appendBeforeQueryString, querystring)
                => GetUriForOpenAi(type, appendBeforeQueryString, querystring, uriHelper);
        }
        private static string GetUriForOpenAi(OpenAiType type,
           string appendBeforeQueryString,
           Dictionary<string, string>? querystring,
           UriHelperConfigurator uriHelper)
        {
            return type switch
            {
                OpenAiType.AudioTranscription => string.Format(uriHelper.AudioTranscriptionUri, appendBeforeQueryString),
                OpenAiType.AudioTranslation => string.Format(uriHelper.AudioTranslationUri, appendBeforeQueryString),
                OpenAiType.AudioSpeech => string.Format(uriHelper.AudioSpeechUri, appendBeforeQueryString),
                OpenAiType.Moderation => string.Format(uriHelper.ModerationUri, appendBeforeQueryString),
                OpenAiType.Image => string.Format(uriHelper.ImageUri, appendBeforeQueryString),
                OpenAiType.Embedding => string.Format(uriHelper.EmbeddingUri, appendBeforeQueryString),
                OpenAiType.Chat => string.Format(uriHelper.ChatUri, appendBeforeQueryString),
                OpenAiType.File => string.Format(uriHelper.FileUri, appendBeforeQueryString),
                OpenAiType.Upload => string.Format(uriHelper.UploadUri, appendBeforeQueryString),
                OpenAiType.FineTuning => string.Format(uriHelper.FineTuneUri, appendBeforeQueryString),
                OpenAiType.Billing => string.Format(uriHelper.BillingUri, appendBeforeQueryString),
                OpenAiType.Assistant => PassForQuerystringCheck(querystring, string.Format(uriHelper.AssistantUri, appendBeforeQueryString)),
                _ => string.Format(uriHelper.ModelUri, appendBeforeQueryString),
            };

        }
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
        private const string QuestionMark = "?";
        private const string CognitiveServicesDomain = "cognitiveservices.azure.com";
        private const string OpenAiDomain = "openai.azure.com";

        private void ConfigureEndpointsForAzure()
        {
            var uriHelper = new UriHelperConfigurator();
            WithAzure = true;
            SetManagedIdentityForAzure();

            var uris = new Dictionary<string, string>();
            Settings.Version ??= "2022-12-01";
            foreach (var deployment in Settings.Deployments)
            {
                if (!deployment.Value.ContainsKey("{Forced}"))
                {
                    deployment.Value.Add("{Forced}", Forced);
                }
            }
            Enum.GetValues<OpenAiType>().ForEach(currentType =>
            {
                if (!Settings.Deployments.TryGetValue(currentType, out var value))
                {
                    value = [];
                    Settings.Deployments.Add(currentType, value);
                }
                if (!value.ContainsKey("{Forced}"))
                {
                    value.Add("{Forced}", Forced);
                }
            });
            uriHelper.ModelUri = string.Format(uriHelper.ModelUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.Model)}");
            uriHelper.FineTuneUri = string.Format(uriHelper.FineTuneUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.FineTuning)}");
            uriHelper.FileUri = string.Format(uriHelper.FileUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.File)}");
            uriHelper.UploadUri = string.Format(uriHelper.UploadUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.Upload)}");
            uriHelper.BillingUri = string.Format(uriHelper.BillingUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.Billing)}");
            uriHelper.DeploymentUri = string.Format(uriHelper.DeploymentUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.Deployment)}");
            uriHelper.AssistantUri = string.Format(uriHelper.AssistantUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.Assistant)}");

            foreach (var deployments in Settings.Deployments)
            {
                foreach (var deployment in deployments.Value)
                {
                    var key = deployment.Key.Contains(Forced) ? "{0}" : deployment.Key;
                    uris.TryAdd($"{deployment.Value.Name}_{deployments.Key}",
                        $"{string.Format(GetDeploymentTypeUri(), $"https://{Settings.Azure.ResourceName}.{GetDeploymentTypeDomain()}/openai/deployments/{key}", "{1}", $"?api-version={GetVersion(Settings, deployments.Key)}")}");
                    string GetDeploymentTypeDomain()
                    {
                        return deployments.Key switch
                        {
                            OpenAiType.Image => CognitiveServicesDomain,
                            _ => OpenAiDomain
                        };
                    }
                    string GetDeploymentTypeUri()
                    {
                        return deployments.Key switch
                        {
                            OpenAiType.Chat => uriHelper.ChatUri,
                            OpenAiType.Embedding => uriHelper.EmbeddingUri,
                            OpenAiType.Moderation => uriHelper.ModerationUri,
                            OpenAiType.Image => uriHelper.ImageUri,
                            OpenAiType.AudioTranscription => uriHelper.AudioTranscriptionUri,
                            OpenAiType.AudioTranslation => uriHelper.AudioTranslationUri,
                            OpenAiType.AudioSpeech => uriHelper.AudioSpeechUri,
                            OpenAiType.File => uriHelper.FileUri,
                            OpenAiType.Upload => uriHelper.UploadUri,
                            OpenAiType.FineTuning => uriHelper.FineTuneUri,
                            OpenAiType.Billing => uriHelper.BillingUri,
                            OpenAiType.Model => uriHelper.ModelUri,
                            OpenAiType.Deployment => uriHelper.DeploymentUri,
                            OpenAiType.Assistant => uriHelper.AssistantUri,
                            _ => throw new NotImplementedException(),
                        };
                    }
                }
            }

            GetUri = (type, modelId, forceModel, appendBeforeQueryString, querystring)
                => GetUriForAzure(type, modelId, forceModel, appendBeforeQueryString, querystring, uriHelper, uris);
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
        private static string GetUriForAzure(OpenAiType type,
            string modelId, bool forceModel,
            string appendBeforeQueryString,
            Dictionary<string, string>? querystring,
            UriHelperConfigurator uriHelper,
            Dictionary<string, string> uris)
        {
            switch (type)
            {
                case OpenAiType.File:
                    return string.Format(uriHelper.FileUri, appendBeforeQueryString);
                case OpenAiType.Upload:
                    return string.Format(uriHelper.UploadUri, appendBeforeQueryString);
                case OpenAiType.FineTuning:
                    return string.Format(uriHelper.FineTuneUri, appendBeforeQueryString);
                case OpenAiType.Billing:
                    return string.Format(uriHelper.BillingUri, appendBeforeQueryString);
                case OpenAiType.Model:
                    return string.Format(uriHelper.ModelUri, appendBeforeQueryString);
                case OpenAiType.Deployment:
                    return string.Format(uriHelper.DeploymentUri, appendBeforeQueryString);
                case OpenAiType.Assistant:
                    return PassForQuerystringCheck(querystring, string.Format(uriHelper.AssistantUri, appendBeforeQueryString));
            }
            if (forceModel)
                return string.Format(uris[$"{Forced}_{type}"], modelId, appendBeforeQueryString);
            var key = $"{modelId}_{type}";
            if (uris.TryGetValue(key, out var value))
                return string.Format(value, string.Empty, appendBeforeQueryString);
            else if (uris.TryGetValue($"{Asterisk}_{type}", out var asteriskValue))
                return string.Format(asteriskValue, string.Empty, appendBeforeQueryString);
            else
                return string.Format(uris[$"{Forced}_{type}"], modelId, appendBeforeQueryString);
        }
        internal const string Asterisk = "*";
        private static string? GetVersion(OpenAiSettings settings, OpenAiType type)
        {
            if (settings.Versions.TryGetValue(type, out var value))
                return value;
            else
                return settings.Version;
        }
    }
}
