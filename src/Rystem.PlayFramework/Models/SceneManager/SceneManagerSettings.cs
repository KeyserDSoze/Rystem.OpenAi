namespace Rystem.PlayFramework
{
    public sealed class SceneManagerSettings
    {
        public SceneManagerOpenAiSettings OpenAi { get; } = new SceneManagerOpenAiSettings();

        /// <summary>
        /// Planning settings for multi-scene orchestration.
        /// </summary>
        public PlanningSettings Planning { get; } = new PlanningSettings();

        /// <summary>
        /// Summarization settings for conversation history.
        /// </summary>
        public SummarizationSettings Summarization { get; } = new SummarizationSettings();
    }

    public sealed class PlanningSettings
    {
        /// <summary>
        /// Enable or disable automatic planning. Default is true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Maximum number of scenes to include in a plan. Default is 10.
        /// </summary>
        public int MaxScenesInPlan { get; set; } = 10;
    }

    public sealed class SummarizationSettings
    {
        /// <summary>
        /// Enable or disable automatic summarization. Default is true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Threshold for number of responses before triggering summarization. Default is 50.
        /// </summary>
        public int ResponseThreshold { get; set; } = 50;

        /// <summary>
        /// Threshold for total character count before triggering summarization. Default is 10000.
        /// </summary>
        public int CharacterThreshold { get; set; } = 10000;
    }
}
