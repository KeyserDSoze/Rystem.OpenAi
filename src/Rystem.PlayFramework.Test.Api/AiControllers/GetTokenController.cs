using Microsoft.AspNetCore.Mvc;
using Rystem.OpenAi;

namespace Rystem.PlayFramework.Test.Api.AiControllers
{
    /// <summary>
    /// Get a token
    /// </summary>
    [Route("api/gettoken")]
    public class GetTokenController : Controller
    {
        private readonly IOpenAi _openAi;
        /// <summary>
        /// Get a token constructor
        /// </summary>
        /// <param name="openAi"></param>
        public GetTokenController(IFactory<IOpenAi> openAi)
        {
            _openAi = openAi.Create("openai")!;
        }
        [HttpGet]
        /// <summary>
        /// Get a token
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> IndexAsync()
        {
            var session = await _openAi.RealTime.CreateSessionAsync();
            return Ok(session);
        }
    }
}
