﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;

[assembly: InternalsVisibleTo("Rystem.OpenAi.Test")]
namespace Rystem.OpenAi
{
    internal delegate string OpenaiUriMaker(OpenAiType type, string modelId, bool forced, string appendBeforeQueryString);
    public sealed class OpenAiConfiguration
    {
        private const string Forced = nameof(Forced);
        internal OpenaiUriMaker GetUri { get; private set; } = null!;
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
            public string FineTuneUri { get; set; } = string.Format(BaseUri, "{0}", "fine_tuning/jobs", "{1}", "{2}");
            public string ModelUri { get; set; } = string.Format(BaseUri, "{0}", "models", "{1}", "{2}");
            public string ModerationUri { get; set; } = string.Format(BaseUri, "{0}", "moderations", "{1}", "{2}");
            public string ImageUri { get; set; } = string.Format(BaseUri, "{0}", "images", "{1}", "{2}");
            public string AudioTranscriptionUri { get; set; } = string.Format(BaseUri, "{0}", "audio/transcriptions", "{1}", "{2}");
            public string AudioTranslationUri { get; set; } = string.Format(BaseUri, "{0}", "audio/translations", "{1}", "{2}");
            public string AudioSpeechUri { get; set; } = string.Format(BaseUri, "{0}", "audio/speech", "{1}", "{2}");
            public string BillingUri { get; set; } = string.Format(BaseUri, "{0}", "dashboard/billing/usage", "{1}", "{2}");
            public string DeploymentUri { get; set; } = string.Format(BaseUri, "{0}", "deployments", "{1}", "{2}");
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
            uriHelper.FineTuneUri = string.Format(uriHelper.FineTuneUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.FineTuning)}", "{0}", string.Empty);
            uriHelper.ModelUri = string.Format(uriHelper.ModelUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.Model)}", "{0}", string.Empty);
            uriHelper.ModerationUri = string.Format(uriHelper.ModerationUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.Moderation)}", "{0}", string.Empty);
            uriHelper.ImageUri = string.Format(uriHelper.ImageUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.Image)}", "{0}", string.Empty);
            uriHelper.AudioTranscriptionUri = string.Format(uriHelper.AudioTranscriptionUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.AudioTranscription)}", "{0}", string.Empty);
            uriHelper.AudioTranslationUri = string.Format(uriHelper.AudioTranslationUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.AudioTranslation)}", "{0}", string.Empty);
            uriHelper.AudioSpeechUri = string.Format(uriHelper.AudioSpeechUri, $"https://api.openai.com/{GetVersion(Settings, OpenAiType.AudioSpeech)}", "{0}", string.Empty);
            uriHelper.BillingUri = string.Format(uriHelper.BillingUri, $"https://api.openai.com", "{0}", string.Empty);

            GetUri = (type, modelId, forceModel, appendBeforeQueryString)
                => GetUriForOpenAi(type, appendBeforeQueryString, uriHelper);
        }
        private static string GetUriForOpenAi(OpenAiType type,
           string appendBeforeQueryString,
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
                OpenAiType.FineTuning => string.Format(uriHelper.FineTuneUri, appendBeforeQueryString),
                OpenAiType.Billing => string.Format(uriHelper.BillingUri, appendBeforeQueryString),
                _ => string.Format(uriHelper.ModelUri, appendBeforeQueryString),
            };
        }
        private void ConfigureEndpointsForAzure()
        {
            var uriHelper = new UriHelperConfigurator();
            WithAzure = true;
            SetManagedIdentityForAzure();

            var uris = new Dictionary<string, string>();
            Settings.Version ??= "2022-12-01";
            if (!Settings.Deployments.ContainsKey("{ChatForced}"))
            {
                Settings.Deployments.Add("{ChatForced}", new OpenAiSettings.DeploymentType { Name = Forced, Type = OpenAiType.Chat });
                Settings.Deployments.Add("{EmbeddingForced}", new OpenAiSettings.DeploymentType { Name = Forced, Type = OpenAiType.Embedding });
                Settings.Deployments.Add("{ModerationForced}", new OpenAiSettings.DeploymentType { Name = Forced, Type = OpenAiType.Moderation });
                Settings.Deployments.Add("{ImageForced}", new OpenAiSettings.DeploymentType { Name = Forced, Type = OpenAiType.Image });
                Settings.Deployments.Add("{AudioTranscrptionForced}", new OpenAiSettings.DeploymentType { Name = Forced, Type = OpenAiType.AudioTranscription });
                Settings.Deployments.Add("{AudioTranslationForced}", new OpenAiSettings.DeploymentType { Name = Forced, Type = OpenAiType.AudioTranslation });
                Settings.Deployments.Add("{AudioSpeechForced}", new OpenAiSettings.DeploymentType { Name = Forced, Type = OpenAiType.AudioSpeech });
            }

            uriHelper.ModelUri = string.Format(uriHelper.ModelUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.Model)}");
            uriHelper.FineTuneUri = string.Format(uriHelper.FineTuneUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.FineTuning)}");
            uriHelper.FileUri = string.Format(uriHelper.FileUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.File)}");
            uriHelper.BillingUri = string.Format(uriHelper.BillingUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.Billing)}");
            uriHelper.DeploymentUri = string.Format(uriHelper.DeploymentUri, $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai", "{0}", $"?api-version={GetVersion(Settings, OpenAiType.Billing)}");

            foreach (var deployment in Settings.Deployments)
            {
                var key = deployment.Key.Contains(Forced) ? "{0}" : deployment.Key;
                uris.TryAdd($"{deployment.Value.Name}_{deployment.Value.Type}",
                    $"{string.Format(GetDeploymentTypeUri(), $"https://{Settings.Azure.ResourceName}.openai.azure.com/openai/deployments/{key}", "{1}", $"?api-version={GetVersion(Settings, deployment.Value.Type)}")}");

                string GetDeploymentTypeUri()
                {
                    return deployment.Value.Type switch
                    {
                        OpenAiType.Chat => uriHelper.ChatUri,
                        OpenAiType.Embedding => uriHelper.EmbeddingUri,
                        OpenAiType.Moderation => uriHelper.ModerationUri,
                        OpenAiType.Image => uriHelper.ImageUri,
                        OpenAiType.AudioTranscription => uriHelper.AudioTranscriptionUri,
                        OpenAiType.AudioTranslation => uriHelper.AudioTranslationUri,
                        OpenAiType.AudioSpeech => uriHelper.AudioSpeechUri,
                        _ => throw new NotImplementedException(),
                    };
                }
            }

            GetUri = (type, modelId, forceModel, appendBeforeQueryString)
                => GetUriForAzure(type, modelId, forceModel, appendBeforeQueryString, uriHelper, uris);
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
            UriHelperConfigurator uriHelper,
            Dictionary<string, string> uris)
        {
            switch (type)
            {
                case OpenAiType.File:
                    return string.Format(uriHelper.FileUri, appendBeforeQueryString);
                case OpenAiType.FineTuning:
                    return string.Format(uriHelper.FineTuneUri, appendBeforeQueryString);
                case OpenAiType.Billing:
                    return string.Format(uriHelper.BillingUri, appendBeforeQueryString);
                case OpenAiType.Model:
                    return string.Format(uriHelper.ModelUri, appendBeforeQueryString);
                case OpenAiType.Deployment:
                    return string.Format(uriHelper.DeploymentUri, appendBeforeQueryString);
            }
            if (forceModel)
                return string.Format(uris[$"{Forced}_{type}"], modelId, appendBeforeQueryString);
            var key = $"{modelId}_{type}";
            if (uris.TryGetValue(key, out var value))
                return string.Format(value, string.Empty, appendBeforeQueryString);
            else if (uris.TryGetValue($"{Asterisk}_{type}", out var asteriskValue))
                return string.Format(asteriskValue, string.Empty, appendBeforeQueryString);
            else
                throw new ArgumentException($"Model {modelId} of {type} is not installed during startup phase. In services.AddOpenAi configure the Azure environment with the correct DeploymentModel.");
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
