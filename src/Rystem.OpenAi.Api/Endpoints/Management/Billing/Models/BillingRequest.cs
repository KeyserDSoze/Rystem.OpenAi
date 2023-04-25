using System;

namespace Rystem.OpenAi.Management
{
    public sealed class BillingRequest : IOpenAiRequest
    {
        public string? ModelId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
