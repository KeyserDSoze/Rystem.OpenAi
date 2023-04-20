using System;

namespace Rystem.OpenAi.Completion
{
    public interface IOpenAiCompletion
    {
        /// <summary>
        /// Prepare the fluent request for completion api.
        /// </summary>
        /// <param name="prompts">prompt string or array Optional Defaults to<|endoftext|>
        /// The prompt(s) to generate completions for, encoded as a string, array of strings, array of tokens, 
        /// or array of token arrays. Note that <|endoftext|> is the document separator that the model sees during 
        /// training, so if a prompt is not specified the model will generate as if from the beginning of a new document.</param>
        /// <returns></returns>
        CompletionRequestBuilder Request(params string[] prompts);
    }
    [Obsolete("In version 3.x we'll remove IOpenAiCompletionApi and we'll use only IOpenAiCompletion to retrieve services")]
    public interface IOpenAiCompletionApi : IOpenAiCompletion
    {
    }
}
