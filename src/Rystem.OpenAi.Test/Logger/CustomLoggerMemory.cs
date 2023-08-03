using System.Collections.Generic;

namespace Rystem.OpenAi.Test.Logger
{
    public class CustomLoggerMemory
    {
        public List<CustomLog> Logs { get; set; } = new();
    }
}
