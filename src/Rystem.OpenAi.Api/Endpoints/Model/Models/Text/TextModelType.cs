namespace Rystem.OpenAi
{
    public enum TextModelType
    {
        /// <summary>
        /// Ada is usually the fastest model and can perform tasks like parsing text, address correction and certain kinds of classification tasks that don’t require too much nuance. Ada’s performance can often be improved by providing more context.
        /// <b>Good at: Parsing text, simple classification, address correction, keywords</b>
        /// <i>Note: Any task performed by a faster model like Ada can be performed by a more powerful model like Curie or Davinci.</i>
        /// </summary>
        AdaText = 0,
        /// <summary>
        /// Babbage can perform straightforward tasks like simple classification. It’s also quite capable when it comes to Semantic Search ranking how well documents match up with search queries.
        /// <b>Good at: Moderate classification, semantic search classification</b>
        /// </summary>
        BabbageText = 100,
        /// <summary>
        /// Curie is extremely powerful, yet very fast. While Davinci is stronger when it comes to analyzing complicated text, Curie is quite capable for many nuanced tasks like sentiment classification and summarization. Curie is also quite good at answering questions and performing Q&A and as a general service chatbot.
        /// <b>Good at: Language translation, complex classification, text sentiment, summarization</b>
        /// </summary>
        CurieText = 200,
        /// <summary>
        /// Similar capabilities to text-davinci-003 but trained with supervised fine-tuning instead of reinforcement learning. <b>(4000 tokens) Up to Jun 2021</b>.
        /// Davinci is the most capable model family and can perform any task the other models (ada, curie, and babbage) can perform and often with less instruction. For applications requiring a lot of understanding of the content, like summarization for a specific audience and creative content generation, Davinci will produce the best results. These increased capabilities require more compute resources, so Davinci costs more per API call and is not as fast as the other models
        /// Another area where Davinci shines is in understanding the intent of text. Davinci is quite good at solving many kinds of logic problems and explaining the motives of characters. Davinci has been able to solve some of the most challenging AI problems involving cause and effect.
        /// <b>Good at: Complex intent, cause and effect, summarization for audience</b>
        /// </summary>
        DavinciText2 = 301,
        /// <summary>
        /// Can do any language task with better quality, longer output, and consistent instruction-following than the curie, babbage, or ada models. Also supports <see href="https://platform.openai.com/docs/guides/completion/inserting-text">inserting</see> completions within text. <b>(4000 tokens) Up to Jun 2021</b>.
        /// Davinci is the most capable model family and can perform any task the other models (ada, curie, and babbage) can perform and often with less instruction. For applications requiring a lot of understanding of the content, like summarization for a specific audience and creative content generation, Davinci will produce the best results. These increased capabilities require more compute resources, so Davinci costs more per API call and is not as fast as the other models
        /// Another area where Davinci shines is in understanding the intent of text. Davinci is quite good at solving many kinds of logic problems and explaining the motives of characters. Davinci has been able to solve some of the most challenging AI problems involving cause and effect.
        /// <b>Good at: Complex intent, cause and effect, summarization for audience</b>
        /// </summary>
        DavinciText3 = 302,
        /// <summary>
        /// Optimized for code-completion tasks <b>(4000 tokens) Up to Jun 2021</b>.
        /// Most capable Codex model. Particularly good at translating natural language to code. In addition to completing code, also supports <see href="https://platform.openai.com/docs/guides/code/inserting-code">inserting</see> completions within code.
        /// </summary>
        DavinciCode = 400,
        /// <summary>
        /// Almost as capable as Davinci Codex, but slightly faster. This speed advantage may make it preferable for real-time applications.
        /// <b>Up to 2,048 tokens</b>
        /// </summary>
        CushmanCode = 500,
    }
}
