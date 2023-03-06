using System.Collections.Generic;
using Rystem.OpenAi.Models;

namespace Rystem.OpenAi
{
    public sealed class OpenAiAzureSettings
    {
        internal bool HasConfiguration => ResourceName != null;
        public string? ResourceName { get; set; }
        internal Dictionary<string, string> Deployments { get; } = new Dictionary<string, string>();
        public OpenAiAzureSettings AddDeploymentModel(string name, TextModelType model)
        {
            Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentModel(string name, EmbeddingModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentModel(string name, AudioModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentModel(string name, ChatModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentModel(string name, EditModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
        public OpenAiAzureSettings AddDeploymentModel(string name, ModerationModelType model)
        {
            if (!Deployments.ContainsKey(name))
                Deployments.Add(name, model.ToModel().Id!);
            return this;
        }
    }
}
