using System.Collections.Generic;

namespace Rystem.OpenAi
{
    public sealed class OpenAiPriceList
    {
        public static OpenAiPriceList Instance { get; } = new OpenAiPriceList();
        private OpenAiPriceList() { }
        public Dictionary<string, OpenAiPriceSettings> Prices { get; set; } = new Dictionary<string, OpenAiPriceSettings>();
    }
}
