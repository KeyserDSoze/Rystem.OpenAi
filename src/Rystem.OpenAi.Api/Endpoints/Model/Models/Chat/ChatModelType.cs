namespace Rystem.OpenAi
{
    public enum ChatModelType
    {
        /// <summary>
        /// Turbo is the same model family that powers ChatGPT. It is optimized for conversational chat input and output but does equally well on completions when compared with the Davinci model family. Any use case that can be done well in ChatGPT should perform well with the Turbo model family in the API.
        /// </summary>
        Gpt35Turbo = 350,
        /// <summary>
        /// Turbo is the same model family that powers ChatGPT. It is optimized for conversational chat input and output but does equally well on completions when compared with the Davinci model family. Any use case that can be done well in ChatGPT should perform well with the Turbo model family in the API.
        /// </summary>
        Gpt35Turbo0301 = 351,
        /// <summary>
        /// More capable than any GPT-3.5 model, able to do more complex tasks, and optimized for chat. Will be updated with our latest model iteration.
        /// 8,192 tokens.
        /// </summary>
        Gpt4 = 400,
        /// <summary>
        /// Same capabilities as the base gpt-4 mode but with 4x the context length. Will be updated with our latest model iteration.
        /// 32,768 tokens.
        /// </summary>
        Gpt4_32K = 401,
        /// <summary>
        /// Snapshot of gpt-4 from March 14th 2023. Unlike gpt-4, this model will not receive updates, and will only be supported for a three month period ending on June 14th 2023.
        /// 8,192 tokens.
        /// </summary>
        Gpt4_0314 = 403,
        /// <summary>
        /// Snapshot of gpt-4-32 from March 14th 2023. Unlike gpt-4-32k, this model will not receive updates, and will only be supported for a three month period ending on June 14th 2023.
        /// 32,768 tokens.
        /// </summary>
        Gpt4_32K_0314 = 404,
    }
}
