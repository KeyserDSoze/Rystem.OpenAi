using System.Collections.Generic;
using Rystem.OpenAi;

namespace Rystem.OpenAi
{
    public sealed class OpenAiAzureSettings
    {
        internal bool HasConfiguration => ResourceName != null;
        public string? ResourceName { get; set; }
        internal Dictionary<string, string> Deployments { get; } = new Dictionary<string, string>();
        public OpenAiAzureSettings AddDeploymentTextModel(string name, TextModelType model)
        {
            Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentEmbeddingModel(string name, EmbeddingModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentAudioModel(string name, AudioModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentChatModel(string name, ChatModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentEditModel(string name, EditModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentModerationModel(string name, ModerationModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentCustomModel(string name, string customeModelId)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, customeModelId);
            return this;
        }
    }
}
