namespace Rystem.OpenAi.Completion
{
    public interface IOpenAiCompletionApi
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
}
