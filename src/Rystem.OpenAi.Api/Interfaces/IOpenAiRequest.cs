using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public interface IOpenAiRequest
    {
        string? ModelId { get; set; }
    }
}
