namespace Rystem.PlayFramework;

public sealed class CacheSettings
{
    public TimeSpan ExpirationDefault { get; set; } = TimeSpan.FromMinutes(15);
    public AiResponseStatus RecordedResponses { get; set; } = AiResponseStatus.Request | AiResponseStatus.Running;
}
