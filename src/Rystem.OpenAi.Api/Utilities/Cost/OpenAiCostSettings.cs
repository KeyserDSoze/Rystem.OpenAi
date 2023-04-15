using System.Collections.Generic;
using Rystem.OpenAi.Image;

namespace Rystem.OpenAi
{
    public sealed class OpenAiCostSettings
    {
        internal Dictionary<string, decimal> Settings { get; } = new Dictionary<string, decimal>();
        internal OpenAiCostSettings()
        {
            SetGpt4With8KPrice();
            SetGpt4With32KPrice();
            SetGpt3_5();
            SetAda();
            SetBabbage();
            SetCurie();
            SetDavinci();
            SetFineTuneForAda();
            SetFineTuneForBabbage();
            SetFineTuneForCurie();
            SetFineTuneForDavinci();
            SetImage(ImageSize.Large, 0.02M);
            SetImage(ImageSize.Medium, 0.018M);
            SetImage(ImageSize.Small, 0.016M);
            SetAudio();
        }
        public OpenAiCostSettings SetGpt4With8KPrice(decimal prompt = 0.03M, decimal completion = 0.06M)
        {
            return this;
        }
        public OpenAiCostSettings SetGpt4With32KPrice(decimal prompt = 0.06M, decimal completion = 0.12M)
        {
            return this;
        }
        public OpenAiCostSettings SetGpt3_5(decimal usage = 0.002M)
        {
            return this;
        }
        public OpenAiCostSettings SetAda(decimal usage = 0.0004M)
        {
            return this;
        }
        public OpenAiCostSettings SetBabbage(decimal usage = 0.0005M)
        {
            return this;
        }
        public OpenAiCostSettings SetCurie(decimal usage = 0.002M)
        {
            return this;
        }
        public OpenAiCostSettings SetDavinci(decimal usage = 0.02M)
        {
            return this;
        }
        public OpenAiCostSettings SetFineTuneForAda(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return this;
        }
        public OpenAiCostSettings SetFineTuneForBabbage(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return this;
        }
        public OpenAiCostSettings SetFineTuneForCurie(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return this;
        }
        public OpenAiCostSettings SetFineTuneForDavinci(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return this;
        }
        public OpenAiCostSettings SetImage(ImageSize size, decimal perUnit)
        {
            return this;
        }
        public OpenAiCostSettings SetAudio(decimal perMinute = 0.006M)
        {
            return this;
        }
    }
}
