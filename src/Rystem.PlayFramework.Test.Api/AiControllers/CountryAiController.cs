using Microsoft.AspNetCore.Components;

namespace Rystem.PlayFramework.Test.Api
{
    /// <summary>
    /// Controller for country AI.
    /// </summary>
    [Route("[controller]/[action]")]
    public class CountryAiController : AiBaseController
    {
        /// <summary>
        /// Something async.
        /// </summary>
        /// <returns></returns>
        public async Task SomethingAsync()
        {
            await Task.CompletedTask;
        }
    }
}
