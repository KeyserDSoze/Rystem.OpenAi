namespace Rystem.PlayFramework.Test.Api.Services
{
    public sealed class IdentityManager
    {
        public async Task<string> GetNameAsync(string username)
        {
            await Task.Delay(0);
            return "Alessandro Rapiti";
        }
    }
}
