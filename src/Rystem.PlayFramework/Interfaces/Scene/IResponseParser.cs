namespace Rystem.PlayFramework
{
    public interface IResponseParser
    {
        string? ParseResponse(object? response);
    }
}
