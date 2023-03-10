namespace Rystem.OpenAi
{
    public sealed class OpenAiAzureAppRegistrationSettings
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? TenantId { get; set; }
        public string[]? Scopes { get; set; }
    }
}
