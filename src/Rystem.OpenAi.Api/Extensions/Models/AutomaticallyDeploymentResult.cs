using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public sealed class AutomaticallyDeploymentResult
    {
        public string? IntegrationName { get; set; }
        public string? Description { get; set; }
        public Exception? Exception { get; set; }
        public bool HasException => Exception != null;
    }
}
