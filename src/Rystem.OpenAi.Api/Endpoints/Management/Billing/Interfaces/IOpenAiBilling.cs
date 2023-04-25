using System;

namespace Rystem.OpenAi.Management
{
    public interface IOpenAiBilling
    {
        BillingBuilder From(DateTime? from = null);
    }
}
