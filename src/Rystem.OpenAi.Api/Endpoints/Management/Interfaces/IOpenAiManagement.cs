using System;
using System.Reflection;

namespace Rystem.OpenAi.Management
{
    public interface IOpenAiManagement
    {
        /// <summary>
        /// Prepare request to retrieve billing data for your account.
        /// </summary>
        /// <returns>Billing</returns>
        BillingBuilder Billing();
        /// <summary>
        /// This methods are available only for Azure integration. A deployment represents a model you can use in your Azure OpenAi environment.
        /// You cannot use that model without a deployment of it. You have choose capacity and scale type of your model.
        /// </summary>
        /// <returns></returns>
        DeploymentBuilder Deployment();
    }
    [Obsolete("In version 3.x we'll remove IOpenAiManagementApi and we'll use only IOpenAiManagement to retrieve services")]
    public interface IOpenAiManagementApi : IOpenAiManagement
    {
    }
}
