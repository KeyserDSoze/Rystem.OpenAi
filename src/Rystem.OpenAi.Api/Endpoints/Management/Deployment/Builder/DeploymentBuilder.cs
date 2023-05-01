using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Management
{
    /// <summary>
    /// This methods are available only for Azure integration. A deployment represents a model you can use in your Azure OpenAi environment.
    /// You cannot use that model without a deployment of it. You have choose capacity and scale type of your model.
    /// </summary>
    public sealed class DeploymentBuilder : RequestBuilder<DeploymentRequest>
    {
        private readonly string? _deploymentId;
        private readonly bool _isUpdate;
        public DeploymentBuilder(HttpClient client, OpenAiConfiguration configuration, IOpenAiUtility utility, string? deploymentId, bool isUpdate) :
            base(client, configuration, () =>
            {
                return new DeploymentRequest()
                {
                    ScaleSettings = new DeploymentScaleSettings
                    {
                        Capacity = 1,
                        ScaleType = "manual"
                    }
                };
            }, utility)
        {
            _deploymentId = deploymentId;
            _isUpdate = isUpdate;
        }
        public DeploymentBuilder WithDeploymentTextModel(string name, TextModelType model)
        {
            Request.ModelId = model.ToModelId();
            return this;
        }
        public DeploymentBuilder WithDeploymentEmbeddingModel(string name, EmbeddingModelType model)
        {
            Request.ModelId = model.ToModelId();
            return this;
        }
        public DeploymentBuilder WithDeploymentAudioModel(string name, AudioModelType model)
        {
            Request.ModelId = model.ToModelId();
            return this;
        }
        public DeploymentBuilder WithDeploymentChatModel(string name, ChatModelType model)
        {
            Request.ModelId = model.ToModelId();
            return this;
        }
        public DeploymentBuilder WithDeploymentEditModel(string name, EditModelType model)
        {
            Request.ModelId = model.ToModelId();
            return this;
        }
        public DeploymentBuilder WithDeploymentModerationModel(string name, ModerationModelType model)
        {
            Request.ModelId = model.ToModelId();
            return this;
        }
        public DeploymentBuilder WithDeploymentCustomModel(string name, string customeModelId)
        {
            Request.ModelId = customeModelId;
            return this;
        }
        /// <summary>
        /// The constant reserved capacity of the inference endpoint for this deployment.
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public DeploymentBuilder WithCapacity(int capacity)
        {
            Request.ScaleSettings.Capacity = capacity;
            return this;
        }
        /// <summary>
        /// Defines how scaling operations will be executed.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DeploymentBuilder WithScaling(DeploymentScaleType type)
        {
            Request.ScaleSettings.ScaleType = type.ToString().ToLower();
            return this;
        }
        /// <summary>
        /// Execute an operation of Creation or Update for the Azure OpenAI resource according to the given specification.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public ValueTask<DeploymentResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (!_isUpdate)
            {
                if (_deploymentId == null)
                    return Client.PostAsync<DeploymentResult>(Configuration.GetUri(OpenAiType.Deployment, string.Empty, _forced, string.Empty),
                        Request, Configuration, cancellationToken);
                else
                    return Client.PutAsync<DeploymentResult>(Configuration.GetUri(OpenAiType.Deployment, string.Empty, _forced, $"/{_deploymentId}"),
                        Request, Configuration, cancellationToken);
            }
            else
            {
                return Client.PatchAsync<DeploymentResult>(Configuration.GetUri(OpenAiType.Deployment, string.Empty, _forced, $"/{_deploymentId}"),
                    Request, Configuration, cancellationToken);
            }
        }
    }
}
