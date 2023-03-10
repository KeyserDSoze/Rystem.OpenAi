using System.Collections.Generic;

namespace Rystem.OpenAi
{
    public sealed class OpenAiAzureSettings
    {
        internal bool HasConfiguration => ResourceName != null;
        public string? ResourceName { get; set; }
        internal bool HasAnotherKindOfAuthentication => HasManagedIdentity || HasAppRegistration;
        internal bool HasManagedIdentity => ManagedIdentity.Id != null || ManagedIdentity.UseDefault;
        internal bool HasAppRegistration => AppRegistration.ClientId != null;
        /// <summary>
        /// Configure managed identity.
        /// Learn how to use this identity on the <see href="https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview">Microsoft docs</see>.
        /// </summary>
        public OpenAiAzureManagedIdentitySettings ManagedIdentity { get; } = new OpenAiAzureManagedIdentitySettings();
        /// <summary>
        /// Configure an app registration.
        /// Learn how to use this identity on the <see href="https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app">Microsoft docs</see>.
        /// </summary>
        public OpenAiAzureAppRegistrationSettings AppRegistration { get; } = new OpenAiAzureAppRegistrationSettings();
        internal Dictionary<string, string> Deployments { get; } = new Dictionary<string, string>();
        public OpenAiAzureSettings AddDeploymentTextModel(string name, TextModelType model)
        {
            Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentEmbeddingModel(string name, EmbeddingModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentAudioModel(string name, AudioModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentChatModel(string name, ChatModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentEditModel(string name, EditModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentModerationModel(string name, ModerationModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentCustomModel(string name, string customeModelId)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, customeModelId);
            return this;
        }
    }
}
