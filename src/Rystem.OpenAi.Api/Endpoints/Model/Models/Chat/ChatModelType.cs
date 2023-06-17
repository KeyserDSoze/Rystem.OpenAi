namespace Rystem.OpenAi
{
    public enum ChatModelType
    {
        /// <summary>
        /// Turbo is the same model family that powers ChatGPT. It is optimized for conversational chat input and output but does equally well on completions when compared with the Davinci model family. Any use case that can be done well in ChatGPT should perform well with the Turbo model family in the API.
        /// </summary>
        Gpt35Turbo = 350,
        /// <summary>
        /// Snapshot of gpt-3.5-turbo from June 13th 2023 with function calling data. Unlike gpt-3.5-turbo, this model will not receive updates, and will be deprecated 3 months after a new version is released.
        /// </summary>
        Gpt35Turbo_Snapshot = 351,
        /// <summary>
        /// Same capabilities as the standard gpt-3.5-turbo model but with 4 times the context.
        /// </summary>
        Gpt35Turbo_16K = 352,
        /// <summary>
        /// Snapshot of gpt-3.5-turbo-16k from June 13th 2023. Unlike gpt-3.5-turbo-16k, this model will not receive updates, and will be deprecated 3 months after a new version is released.
        /// </summary>
        Gpt35Turbo_16K_Snapshot = 353,
        /// <summary>
        /// More capable than any GPT-3.5 model, able to do more complex tasks, and optimized for chat. Will be updated with our latest model iteration.
        /// 8,192 tokens.
        /// </summary>
        Gpt4 = 400,
        /// <summary>
        /// Snapshot of gpt-4 from June 13th 2023 with function calling data. Unlike gpt-4, this model will not receive updates, and will be deprecated 3 months after a new version is released.
        /// 8,192 tokens.
        /// </summary>
        Gpt4_Snapshot = 401,
        /// <summary>
        /// Same capabilities as the base gpt-4 mode but with 4x the context length. Will be updated with our latest model iteration.
        /// 32,768 tokens.
        /// </summary>
        Gpt4_32K = 402,
        /// <summary>
        /// Snapshot of gpt-4-32 from June 13th 2023. Unlike gpt-4-32k, this model will not receive updates, and will be deprecated 3 months after a new version is released.
        /// 32,768 tokens.
        /// </summary>
        Gpt4_32K_Snapshot = 403,
    }
}
