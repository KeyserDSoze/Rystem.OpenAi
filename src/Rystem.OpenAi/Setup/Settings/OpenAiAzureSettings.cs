namespace Rystem.OpenAi
{
    public sealed class OpenAiAzureSettings
    {
        internal OpenAiAzureSettings()
        {
        }
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
    }
}
