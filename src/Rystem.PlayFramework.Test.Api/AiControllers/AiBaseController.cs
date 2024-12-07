using Microsoft.AspNetCore.Components;

namespace Rystem.PlayFramework.Test.Api
{
    public abstract class AiBaseController
    {
    }
    [Route("[controller]/[action]")]
    public class CountryAiController : AiBaseController
    {
        public async Task SomethingAsync()
        {

        }
    }
}
