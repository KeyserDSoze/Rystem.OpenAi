using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi.Api.Utilities.Logs
{
    public interface IOpenAiLogger
    {
        ILogger<IOpenAiLogger> Custom { get; }
    }
}
