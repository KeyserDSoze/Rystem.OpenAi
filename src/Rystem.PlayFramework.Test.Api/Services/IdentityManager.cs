namespace Rystem.PlayFramework.Test.Api.Services
{
    /// <summary>
    /// Manages user identities.
    /// </summary>
    public sealed class IdentityManager
    {
        /// <summary>
        /// Gets the name of the user with the specified username.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<string> GetNameAsync(string username)
        {
            await Task.Delay(0);
            _ = username;
            return "Alessandro Rapiti";
        }
    }
}
