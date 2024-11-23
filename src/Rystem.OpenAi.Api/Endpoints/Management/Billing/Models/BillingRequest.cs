using System;

namespace Rystem.OpenAi.Management
{
    public sealed class BillingRequest : IOpenAiRequest
    {
        public string? Model { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
