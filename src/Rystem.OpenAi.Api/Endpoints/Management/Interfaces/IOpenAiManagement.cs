using Rystem.OpenAi.Image;
using System;

namespace Rystem.OpenAi.Management
{
    public interface IOpenAiManagement
    {
        BillingBuilder Billing();
    }
    [Obsolete("In version 3.x we'll remove IOpenAiManagementApi and we'll use only IOpenAiManagement to retrieve services")]
    public interface IOpenAiManagementApi : IOpenAiManagement
    {
    }
}
