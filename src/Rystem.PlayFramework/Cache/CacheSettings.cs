namespace Rystem.PlayFramework
{
    internal sealed class CacheSettings
    {
        public bool MemoryIsDefault { get; set; }
        public bool DistributedIsDefault { get; set; }
        public TimeSpan? ExpirationDefault { get; set; }
    }
}
