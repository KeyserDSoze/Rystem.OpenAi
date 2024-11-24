namespace Rystem.OpenAi.Management
{
    public interface IOpenAiManagement
    {
        /// <summary>
        /// Prepare request to retrieve billing data for your account.
        /// </summary>
        /// <returns>Billing</returns>
        IOpenAiBilling Billing { get; }
        /// <summary>
        /// This methods are available only for Azure integration. A deployment represents a model you can use in your Azure OpenAi environment.
        /// You cannot use that model without a deployment of it. You have choose capacity and scale type of your model.
        /// </summary>
        /// <returns></returns>
        IOpenAiDeployment Deployment { get; }
    }
}
