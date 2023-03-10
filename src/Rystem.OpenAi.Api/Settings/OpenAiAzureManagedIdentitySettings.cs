namespace Rystem.OpenAi
{
    public sealed class OpenAiAzureManagedIdentitySettings
    {
        /// <summary>
        /// Set the ClientId from <see href="https://learn.microsoft.com/en-us/azure/automation/add-user-assigned-identity">user assigned managed identity</see>
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Set true to use the <see href="https://learn.microsoft.com/en-us/azure/automation/enable-managed-identity-for-automation">system assigned managed identity</see>
        /// </summary>
        public bool UseDefault { get; set; }
    }
}
