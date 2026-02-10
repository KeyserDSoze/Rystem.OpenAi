namespace Rystem.PlayFramework
{
    [Flags]
    public enum AiResponseStatus
    {
        Request = 1,
        SceneRequest = 2,
        Running = 4,
        FunctionStreamRequest = 8,
        FunctionRequest = 16,
        FinishedOk = 32,
        FinishedNoTool = 64,
        FinishedWarning = 128,
        FinishedError = 256,
        /// <summary>
        /// Indicates that conversation history summarization is in progress
        /// Used when the response/character threshold is exceeded and the system is condensing previous turns
        /// </summary>
        Summarizing = 1024,
        /// <summary>
        /// Indicates that a tool/function was skipped because it was already executed in the current scene
        /// </summary>
        ToolSkipped = 2048,
    }
}
