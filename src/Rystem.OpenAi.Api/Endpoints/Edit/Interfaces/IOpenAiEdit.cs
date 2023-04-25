using System;

namespace Rystem.OpenAi.Edit
{
    /// <summary>
    /// Given a prompt and an instruction, the model will return an edited version of the prompt.
    /// </summary>
    public interface IOpenAiEdit
    {
        /// <summary>
        /// Given a prompt and an instruction, the model will return an edited version of the prompt.
        /// </summary>
        /// <param name="instruction">The instruction that tells the model how to edit the prompt.</param>
        /// <returns></returns>
        EditRequestBuilder Request(string instruction);
    }
}
