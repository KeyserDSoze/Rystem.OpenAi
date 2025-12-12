using System.Reflection;
using System.Text.Json;

namespace Rystem.PlayFramework
{
    internal sealed class DefaultResponseParser : IResponseParser
    {
        public string? ParseResponse(object? response)
        {
            if (response == null)
                return null;
            if (response.IsPrimitive())
            {
                return response.ToString();
            }
            else
            {
                return response.ToJson();
            }
        }
    }
}
